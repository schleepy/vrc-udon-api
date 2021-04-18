using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace VRCUdonAPI.Services
{
    public interface IImageService
    {
        Image BinaryStringToImage(string binaryString);
        string SaveImageToFile(Image image);
    }
}
