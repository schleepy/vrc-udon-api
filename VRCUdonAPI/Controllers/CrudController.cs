using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Extensions;
using VRCUdonAPI.Services;

namespace VRCUdonAPI.Controllers
{
    public class CrudController : Controller
    {
        protected readonly ImageService ImageService;
        protected readonly VideoService VideoService;

        public CrudController(ImageService imageService, VideoService videoService) 
        {
            ImageService = imageService;
            VideoService = videoService;
        }

        public async Task<FileContentResult> GetEntityAsVideo(object entity)
        {
            Image image = ImageService.GetObjectAsImage(entity);

            string imagePath = ImageService.SaveImageToFile(image);

            string videoPath = await VideoService.CreateVideoFromImage(imagePath);

            byte[] bytes = await System.IO.File.ReadAllBytesAsync(videoPath);
            
            if (ImageService.ImageSettings.AutoDelete)
                System.IO.File.Delete(imagePath);

            if (ImageService.ImageSettings.AutoDelete)
                System.IO.File.Delete(videoPath);
            
            return File(bytes, "video/mp4");
        }
    }
}
