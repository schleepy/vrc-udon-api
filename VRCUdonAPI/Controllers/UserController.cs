using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VRCUdonAPI.Extensions;
using VRCUdonAPI.Helpers;
using VRCUdonAPI.Models.Dtos;
using VRCUdonAPI.Models.Entities;
using VRCUdonAPI.Services;
using User = VRCUdonAPI.Models.Entities.User;

namespace VRCUdonAPI.Controllers
{
    /// <summary>
    /// Example controller for users
    /// </summary>
    [ApiController]
    [Route("users/")]
    public class UserController : CrudController
    {
        private readonly IUserService UserService;

        public UserController(ImageService imageService, VideoService videoService, IUserService userService) : base(imageService, videoService)
        {
            UserService = userService;
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            UserDto user = await UserService.GetSingle(id);
            return await GetEntityAsVideo(user);
        }

        [HttpGet("create/{id}")]
        [HttpGet("create/{id}/{name}")]
        [HttpGet("create/{id}/{name}/{email}")]
        public async Task<IActionResult> Create(string id, string name = "", string email = "")
        {
            #pragma warning disable CS4014
            UserService.Create(new UserDto
            {
                Id = id,
                Name = name,
                Email = email
            });
            #pragma warning restore CS4014

            return await GetEntityAsVideo($"User {id} successfully created");
        }

        [HttpGet("update/{id}/{name}/{email}")]
        public async Task<IActionResult> Update(string id, string name, string email)
        {
            UserService.Update(new UserDto
            {
                Id = id,
                Name = name,
                Email = email
            }, id);

            return await GetEntityAsVideo($"User {id} successfully updated");
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            /*UserDto entity = await UserService.GetSingle(id);
            UserService.Delete(entity);*/
            UserService.Delete(id);

            return await GetEntityAsVideo($"User {id} successfully deleted");
        }
    }
}
