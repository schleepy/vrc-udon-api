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

            services.AddSession(options =>
            {
                options.Cookie.Name = ".VUAAPI.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.IsEssential = true;
            });

            var connectionString = Configuration.GetConnectionString("VSContext");
            services.AddDbContext<VUAContext>(options =>
                options.UseSqlite(connectionString).EnableSensitiveDataLogging());

            // In memory context, perhaps helpful for building queries
            //services.AddDbContext<InMemoryVUAContext>(opt => opt.UseInMemoryDatabase(databaseName: "InMemoryVUADatabase"));

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

            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IQueryService, QueryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
