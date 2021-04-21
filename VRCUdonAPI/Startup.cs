using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Settings;
using VRCUdonAPI.Repositories;
using VRCUdonAPI.Services;
using Xabe.FFmpeg;
using AutoMapper;
using VRCUdonAPI.Middleware;

namespace VRCUdonAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers();

            services.AddDistributedMemoryCache();

            services.AddMvc().AddControllersAsServices();

            services.AddLogging();

            var connectionString = Configuration.GetConnectionString("VSContext");
            services.AddDbContext<VUAContext>(options =>
                options.UseSqlite(connectionString).EnableSensitiveDataLogging());

            services.AddDbContext<QueryContext>(options =>
                options.UseInMemoryDatabase("InMemoryQueryContext"));

            // FFmpeg settings
            services.Configure<FFmpegSettings>(Configuration.GetSection("FFmpeg"));
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<FFmpegSettings>>().Value);

            // Query settings
            services.Configure<QuerySettings>(Configuration.GetSection("Queries"));
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<QuerySettings>>().Value);

            // Video settings
            services.Configure<VideoSettings>(Configuration.GetSection("Video"));
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<VideoSettings>>().Value);

            // Image settings
            services.Configure<ImageSettings>(Configuration.GetSection("Images"));
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<ImageSettings>>().Value);

            services.AddScoped<ImageService>();
            services.AddScoped<VideoService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IQueryService, QueryService>();
            services.AddScoped<IErrorService, ErrorService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleware>();
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ExceptionMiddleware>();

                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
