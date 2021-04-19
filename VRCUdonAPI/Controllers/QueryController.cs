using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Entities;
using VRCUdonAPI.Models.Settings;
using VRCUdonAPI.Services;

namespace VRCUdonAPI.Controllers
{
    [ApiController]
    [Route("query/")]
    public class QueryController : CrudController
    {
        private QuerySettings Settings;
        private IQueryService QueryService;
        private readonly EndpointDataSource EndpointDataSource;

        public QueryController(
            IQueryService queryService,
            ImageService imageService,
            VideoService videoService,
            QuerySettings settings, 
            EndpointDataSource endpointDataSource) : base(imageService, videoService)
        {
            Settings = settings;
            EndpointDataSource = endpointDataSource;
            QueryService = queryService;
        }

        /// <summary>
        /// Builds a query and once the EOQ marker is hit tries to match it with
        /// an existing endpoint and either redirects or returns a dummy video
        /// </summary>
        [Route("build/{*input}")]
        public async Task<IActionResult> Build(string input)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress;

            Query query = await QueryService.Get(address);

            // Check if a query already exists if not build a new one
            if (query == default(Query))
            {
                query = await QueryService.Create(new Query
                {
                    Address = address,
                    Result = input
                });

                return await GetDummyVideo();
            }

            if (input == Settings.EOQMarker)
            {
                QueryService.Delete(query);
                return Run(query.Result);
            }

            query.Result += input;

            await QueryService.Update(query);

            return await GetDummyVideo();
        }

        [HttpGet("dummy")]
        public async Task<IActionResult> GetDummyVideo()
        {
            byte[] bytes = await System.IO.File.ReadAllBytesAsync($"{VideoService.VideoSettings.OutputDirectory}/dumb.mp4");
            return File(bytes, "video/mp4");
        }

        /// <summary>
        /// Searches the environment for a matching endpoint to the query and if found, redirects to it
        /// </summary>
        [HttpGet("run/{**query}")]
        public IActionResult Run(string query)
        {
            var routeEndpoints = EndpointDataSource?.Endpoints.Cast<RouteEndpoint>();
            var routeValues = new RouteValueDictionary();
            string uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{query}";
            string LocalPath = new Uri(uri).LocalPath;

            var matchedEndpoint = routeEndpoints.Where(
                e => new TemplateMatcher(
                TemplateParser.Parse(e.RoutePattern.RawText),
                new RouteValueDictionary())
                .TryMatch(LocalPath, routeValues))
                .OrderBy(c => c.Order)
                .FirstOrDefault();

            if (matchedEndpoint != null)
            {
                string controller = matchedEndpoint.RoutePattern.Defaults["controller"]?.ToString();
                string action = matchedEndpoint.RoutePattern.Defaults["action"]?.ToString();

                // It'd perhaps be better to use the requestdelegate here instead of a redirect but this is simpler
                return RedirectToAction(action, controller, routeValues);
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
