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
using System.Collections.Generic;

namespace CG_OpenCV.Services
{
    internal class CropperService
    {

        public Image<Bgr, Byte> CropBoard(Image<Bgr, Byte> imgOriginal)
        {
            var coords = this.FindBoardCoords(imgOriginal.Copy());

            int width = Math.Abs(coords[0].X - coords[1].X);
            int height = Math.Abs(coords[0].Y - coords[1].Y);

            var croppedImg = imgOriginal.GetSubRect(new Rectangle(coords[0].X, coords[1].Y, width, height));

            string relativePath = Path.Combine("..", "..", "ImagensTeste/fullImageAfterCrop.png");
            string absolutePath = Path.GetFullPath(relativePath);
            croppedImg.Bitmap.Save(absolutePath, ImageFormat.Png);

            return croppedImg;
        }


        public Coord[] FindBoardCoords(Image<Bgr, Byte> imgCopy)
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
                            ImageClass.ColorToHSV(original, out var hue, out var saturation, out var value);
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
        public Coord[] FindPieceCoord(Image<Bgr, Byte> imgCopy)
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
                            var ePreto = blue == 0 && green == 0 && red == 0;

                            if (ePreto) //DAR O TUNE AQUI
                            {
                                histogramY[y] = histogramY[y] + 1;
                                histogramX[x] = histogramX[x] + 1;
                            } 
                        }
                    }
                    int thresholdPertenceTabuleiro = 1; //Alter threshold de percentagem
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
                new Coord(minX,maxX),
                new Coord(minY,maxY),
            };
        }

        public IEnumerable<Image<Bgr, Byte>> CropIndividualImagePieces(Image<Bgr, Byte> croppedBoard)
        {
            var width = croppedBoard.MIplImage.width;
            var height = croppedBoard.MIplImage.height;
            var pieces = new List<Image<Bgr, Byte>>();
            var pieceWidth = width / 8;
            var pieceHeight = height / 8;

            for (int y = 0; y < height; y += pieceHeight)
            {
                for (int x = 0; x < width; x += pieceWidth)
                {
                    if (x + pieceWidth > width) break;
                    var imagemCortadinha = croppedBoard.GetSubRect(new Rectangle(x, y, pieceWidth, pieceHeight));
                    pieces.Add(imagemCortadinha);

                    string relativePath = Path.Combine("..", "..", $"ImagensCortadinhas/x{x}y{y}.png");
                    string absolutePath = Path.GetFullPath(relativePath);
                    imagemCortadinha.Bitmap.Save(absolutePath, ImageFormat.Png);
                }

                if (y + pieceHeight > height) break;
            }

            return pieces;
        }


        public Image<Bgr, Byte> CropFigureFromPieceImage(Image<Bgr, Byte> pieceImage)
        {
            //[0] x min e x max
            //[1] y min e y max
            var coords = this.FindPieceCoord(pieceImage.Copy());

            int width = Math.Abs(coords[0].X - coords[0].Y);
            int height = Math.Abs(coords[1].X - coords[1].Y);

            var croppedImg = pieceImage.GetSubRect(new Rectangle(coords[0].X, coords[1].X, width+1, height+1));

            string relativePath = Path.Combine("..", "..", $"FigurasCortadinhas/{coords[0].X}{coords[1].Y}.png");
            string absolutePath = Path.GetFullPath(relativePath);
            croppedImg.Bitmap.Save(absolutePath, ImageFormat.Png);

            return croppedImg;
        }

    }
}
