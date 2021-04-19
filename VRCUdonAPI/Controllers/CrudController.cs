using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            string binaryStr = entity.ToString().ToBinary();
            var image = ImageService.BinaryStringToImage(binaryStr);
            string imagePath = ImageService.SaveImageToFile(image);
            string videoPath = await VideoService.CreateVideoFromImagesAsync(new List<string>() { imagePath });
            byte[] bytes = await System.IO.File.ReadAllBytesAsync(videoPath);
            // should really add this to a setting
            System.IO.File.Delete(imagePath);
            System.IO.File.Delete(videoPath);
            return File(bytes, "video/mp4");
        }
    }
}
