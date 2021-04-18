using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRCUdonAPI.Services
{
    public interface IVideoService
    {
        Task<string> CreateVideoFromImagesAsync(List<string> images);
        Task<string> CreateVideoFromImage(string image);
    }
}
