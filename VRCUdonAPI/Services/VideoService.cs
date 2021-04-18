using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Settings;
using Xabe.FFmpeg;

namespace VRCUdonAPI.Services
{
    public class VideoService : IVideoService
    {
        private FFmpegSettings FFmpegSettings;
        private VideoSettings VideoSettings;

        public VideoService(FFmpegSettings ffmpegSettings, VideoSettings videoSettings)
        {
            FFmpegSettings = ffmpegSettings;
            VideoSettings = videoSettings;
            FFmpeg.SetExecutablesPath(ffmpegSettings.ExecutablesPath);
        }

        public async Task<string> CreateVideoFromImagesAsync(List<string> images)
        {
            string outputPath = Path.Combine(VideoSettings.OutputDirectory, $"{DateTime.Now.Ticks}.mp4");

            await FFmpeg.Conversions.New()
                .AddParameter("-loop 1")
                .AddParameter($"-i {images.First()}")
                .AddParameter("-c:v libx264")
                .AddParameter($"-t {TimeSpan.FromSeconds(VideoSettings.DurationInSeconds)}")
                .AddParameter("-pix_fmt yuv420p")
                .AddParameter($"-vf scale={VideoSettings.VideoWidth}:{VideoSettings.VideoHeight}")
                .SetOutput(outputPath)
                .Start();

            return outputPath;
        }

        public async Task<string> CreateVideoFromImage(string image)
        {
            return await CreateVideoFromImagesAsync(new List<string>() { image });
        }
    }
}
