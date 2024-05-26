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
using CG_OpenCV.Models;

namespace CG_OpenCV.Services
{
    internal class BoardCroper
    {
        Image<Bgr, Byte> imgCopy = null; // working image
        Image<Bgr, Byte> imgOriginal = null; // working image
        public BoardCroper(Image<Bgr, Byte> img)
        {
            this.imgOriginal = img;
            this.imgCopy = img.Copy();
        }

        public Image<Bgr, Byte> CropBoard()
        {
            var imgCopy = this.imgCopy.Copy();

            var coords = this.FindBoardCoords();

            int width = Math.Abs(coords[0].X - coords[1].X);
            int height = Math.Abs(coords[0].Y - coords[1].Y);

            var croppedImg = this.imgOriginal.GetSubRect(new Rectangle(coords[0].X, coords[1].Y, width, height));

            string relativePath = Path.Combine("..", "..", "ImagensTeste/fullImageAfterCrop.png");
            string absolutePath = Path.GetFullPath(relativePath);
            croppedImg.Bitmap.Save(absolutePath, ImageFormat.Png);

            return imgCopy;
        }

        public void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));
            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }
        public Coord[] FindBoardCoords()
        {
            int minX = -1, maxX = -1, minY = -1, maxY = -1;

            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red;
                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int step = m.widthStep;
                float[] histogramX = new float[width]; //holds the percentage of 255 on that X column
                float[] histogramY = new float[height]; //holds the percentage of 255 of that Y line
                if (nChan == 3) // image in RGB RedGreenBlue
                {

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = (dataPtr + x * nChan + y * step)[0];
                            green = (dataPtr + x * nChan + y * step)[1];
                            red = (dataPtr + x * nChan + y * step)[2];

                            Color original = Color.FromArgb(red, green, blue);
                            ColorToHSV(original, out var hue, out var saturation, out var value);
                            if (150 < hue && hue < 320) //DAR O TUNE AQUI
                            {
                                histogramY[y] = histogramY[y] + 1;
                                histogramX[x] = histogramX[x] + 1;
                                (dataPtr + x * nChan + y * step)[0] = 0;
                                (dataPtr + x * nChan + y * step)[1] = 0;
                                (dataPtr + x * nChan + y * step)[2] = 0;
                            }
                            else
                            {
                                (dataPtr + x * nChan + y * step)[0] = 255;
                                (dataPtr + x * nChan + y * step)[1] = 255;
                                (dataPtr + x * nChan + y * step)[2] = 255;
                            }
                        }
                    }
                    int thresholdPertenceTabuleiro = 3; //Alter threshold de percentagem
                    //Place the values by percentage on the histograms and see if they are the xo,yo or x1,y1 of the board.
                    // Initialize min and max values for X and Y

                    // Process histogramX and find min and max X where histogramX[x] > 20
                    for (x = 0; x < width; x++)
                    {
                        histogramX[x] = (histogramX[x] / height) * 100;
                        if (histogramX[x] > thresholdPertenceTabuleiro)
                        {
                            if (minX == -1)
                            {
                                minX = x;
                            }
                            maxX = x;
                        }
                    }

                    // Process histogramY and find min and max Y where histogramY[y] > 20
                    for (y = 0; y < height; y++)
                    {
                        histogramY[y] = (histogramY[y] / width) * 100;
                        if (histogramY[y] > thresholdPertenceTabuleiro)
                        {
                            if (minY == -1)
                            {
                                minY = y;
                            }
                            maxY = y;
                        }
                    }
                }
            }

            //primeira coordenada ponta inferior esquerda 
            //segunda coordenada ponta superior direita
            return new Coord[2]
            {
                new Coord(minX,maxY),
                new Coord(maxX,minY),
            };
        }
    }
}
