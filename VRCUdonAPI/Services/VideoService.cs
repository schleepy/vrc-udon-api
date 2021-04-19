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
    public class VideoService
    {
        private FFmpegSettings FFmpegSettings;
        public readonly VideoSettings VideoSettings;

        public VideoService(FFmpegSettings ffmpegSettings, VideoSettings videoSettings)
        {
            FFmpegSettings = ffmpegSettings;
            VideoSettings = videoSettings;
            FFmpeg.SetExecutablesPath(ffmpegSettings.ExecutablesPath);

            if (!Directory.Exists(VideoSettings.OutputDirectory))
            {
                Directory.CreateDirectory(VideoSettings.OutputDirectory);
            }
        }

        public async Task<string> CreateVideoFromImages(List<string> images)
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
            return await CreateVideoFromImages(new List<string>() { image });
        }
    }
}
