using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRCUdonAPI.Models.Settings
{
    public class VideoSettings
    {
        public int DurationInSeconds { get; set; }
        public int VideoWidth { get; set; }
        public int VideoHeight { get; set; }
        public string OutputDirectory { get; set; }
    }
}
