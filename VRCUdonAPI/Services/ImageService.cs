using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VRCUdonAPI.Extensions;
using VRCUdonAPI.Models.Settings;

namespace VRCUdonAPI.Services
{
    public class ImageService
    {
        public readonly ImageSettings ImageSettings;

        public ImageService(ImageSettings imageSettings)
        {
            ImageSettings = imageSettings;

            // Check if output directory exists, if not then we create it
            if (!Directory.Exists(ImageSettings.OutputDirectory))
            {
                Directory.CreateDirectory(ImageSettings.OutputDirectory);
            }
        }

        /// <summary>
        /// Converts a string into an ARGB formatted image where each character is represented as decimal
        /// </summary>
        public Image StringToARGBImage(string input)
        {
            if (((ImageSettings.ImageWidth / ImageSettings.BlockSize) % 1) != 0)
            {
                throw new Exception("width needs to be divisible by blocksize");
            }

            Image img = new Bitmap(ImageSettings.ImageWidth, ImageSettings.ImageHeight);
            Graphics drawing = Graphics.FromImage(img);

            int totalBlocks = (int)Math.Ceiling((double)input.Length / 4);
            Console.WriteLine(totalBlocks);

            int x = 0;
            int y = 0;
            for (int i = 0; i < totalBlocks; i++)
            {
                if (x >= ImageSettings.ImageWidth)
                {
                    x = 0;
                    y += ImageSettings.BlockSize;
                }

                decimal[] argb = new decimal[4];

                int startindex = (i * argb.Length);
                int length = startindex + argb.Length < input.Length ? argb.Length : input.Length - startindex;
                var next = input.Substring(startindex, length);

                for (int k = 0; k < next.Length; k++)
                    argb[k] = (decimal)next[k]; // copyTo?

                var color = Color.FromArgb(
                    (int)argb[0],
                    (int)argb[1],
                    (int)argb[2],
                    (int)argb[3]);

                drawing.FillRectangle(new SolidBrush(color), x, y, ImageSettings.BlockSize, ImageSettings.BlockSize);

                x += ImageSettings.BlockSize;
            }

            drawing.Save();
            drawing.Dispose();

            return img;
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

        public Image GetObjectAsImage(object obj)
        {
            if (ImageSettings.ImageType == ImageType.Binary)
            {
                return BinaryStringToImage(obj.ToString().ToBinary());
            }

            if (ImageSettings.ImageType == ImageType.ARGB)
            {
                return StringToARGBImage(obj.ToString());
            }

            return null;
        }

        public string SaveImageToFile(Image image)
        {
            string filePath = Path.Combine($"{DateTime.Now.Ticks}.bmp");
            image.Save(filePath, ImageFormat.Bmp);
            return filePath;
        }
    }
}
