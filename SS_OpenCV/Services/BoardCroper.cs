using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

using Emgu.CV.Structure;
using Emgu.CV;

namespace CG_OpenCV.Services
{
    internal class BoardCroper
    {
        Image<Bgr, Byte> fullImg = null; // working image
        public BoardCroper(Image<Bgr, Byte> img)
        {
            this.fullImg = img.Copy();
        }

        public Image<Bgr, Byte> CropBoard()
        {
            var imgCopy = fullImg.Copy();

            var coords = this.FindBoardCoord();

            int width = coords[1].Value.Item1 - coords[0].Value.Item1;
            int height = coords[1].Value.Item2 - coords[0].Value.Item2;

            var croppedImg = fullImg.GetSubRect(new Rectangle(coords[0].Value.Item1, coords[0].Value.Item2, width, height));

            string relativePath = Path.Combine("..", "..", "ImagensTeste/fullImageAfterCrop.png");
            string absolutePath = Path.GetFullPath(relativePath);
            croppedImg.Bitmap.Save(absolutePath, ImageFormat.Png);

            return imgCopy;
        }

        private (int, int)?[] FindBoardCoord()
        {
            ImageClass.ConvertToBW_Otsu(fullImg);

            //save image locally
            string relativePath = Path.Combine("..", "..", "ImagensTeste/imgbin1.png");
            string absolutePath = Path.GetFullPath(relativePath);

            fullImg.Bitmap.Save(absolutePath, ImageFormat.Png);

            var coordinates = new (int, int)?[2];



            coordinates[0] = FindFirstWhitePixel(fullImg.Bitmap);
            coordinates[1] = FindLastWhitePixel(fullImg.Bitmap);

            return coordinates;
        }

        private (int x, int y)? FindFirstWhitePixel(Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                    {
                        return (x, y);
                    }
                }
            }
            return null;
        }

        private (int x, int y)? FindLastWhitePixel(Bitmap bitmap)
        {
            for (int y = bitmap.Height - 1; y >= 0; y--)
            {
                for (int x = bitmap.Width - 1; x >= 0; x--)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.R == 255 && pixelColor.G == 255 && pixelColor.B == 255)
                    {
                        return (x, y);
                    }
                }
            }
            return null;
        }
    }
}
