using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Models.Settings;

namespace VRCUdonAPI.Services
{
    public class ImageService : IImageService
    {
        private readonly ImageSettings ImageSettings;

        public ImageService(ImageSettings imageSettings)
        {
            ImageSettings = imageSettings;
        }

        public Image BinaryStringToImage(string binaryString)
        {
            int blockSize = ImageSettings.BlockSize;

            Image img = new Bitmap(ImageSettings.ImageWidth, ImageSettings.ImageHeight);
            Graphics drawing = Graphics.FromImage(img);

            if (((ImageSettings.ImageWidth/blockSize) % 1) != 0)
            {
                throw new Exception("width needs to be divisible by blocksize");
            }

            int x = 0;
            int y = 0;
            for (int i = 0; i < binaryString.Length + 1; i++)
            {
                if (i >= binaryString.Length)
                {
                    drawing.FillRectangle(Brushes.Red, x, y, blockSize, blockSize);
                    break;
                }

                char binary = binaryString[i];

                if (x >= ImageSettings.ImageWidth) // ((x + blockSize) >= width)
                {
                    x = 0;
                    y += blockSize;
                }

                if (binary != '0')
                {
                    drawing.FillRectangle(Brushes.White, x, y, blockSize, blockSize);
                } 

                x += blockSize;
            }

            drawing.Save();
            drawing.Dispose();

            return img;
        }

        public string SaveImageToFile(Image image)
        {
            string filePath = Path.Combine($"{DateTime.Now.Ticks}.bmp");
            image.Save(filePath, ImageFormat.Bmp);
            return filePath;
        }
    }
}
