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
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));
            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }
        public static (int x, int y)?[] findBoard(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right
                // aceder diretamente à memória


                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red;
                /*
                 * MIplImage contém vários params que descrevem como está organizada a imagem
                width largura da imagem
                widthStep indica qual a largura de total de uma linha (bytes)
                nChannels indica o número de canais de cor (RGB = 3)
                trabalhar com a estrutura dataptr for necessário avançar ou recuar uma linha
                usar o widthStep
                avançar uma linha de pixeis completa dataPtr += m.widthStep
                avançar numero de pixeis de alinhamento para passar a linha seguinte
                int padding = m.widthStep - m.nChannels * m.width;
                dataPtr += padding;
                */
                int width = img.Width;
                int height = img.Height;
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
                            if (70 < hue && hue < 320) //DAR O TUNE AQUI
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
                    int minX = -1, maxX = -1, minY = -1, maxY = -1;

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
                    // Output the results
                    Console.WriteLine($"Min X: {minX}, Max X: {maxX}");
                    Console.WriteLine($"Min Y: {minY}, Max Y: {maxY}");

                    return 
                    

                }
            }
        }
}
