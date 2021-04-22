using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRCUdonAPI.Models.Settings
{
    public enum ImageType
    {
        Binary,
        ARGB
    }

    public class ImageSettings
    {
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int BlockSize { get; set; }
        public string OutputDirectory { get; set; }
        public bool AutoDelete { get; set; }
        public ImageType ImageType { get; set; } = ImageType.Binary;
    }
}
