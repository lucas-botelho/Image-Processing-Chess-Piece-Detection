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

        public Image<Bgr, Byte> CropBoardAndSaveImage(Image<Bgr, Byte> imgOriginal)
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
            double angleRodar = 0;
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

                int[,] mapaBinarização = new int[width, height]; //vamos encher este mapa com 0s e 1s
                                                                 // Preencher o array com zeros
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        mapaBinarização[i, j] = 0;
                    }
                }
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
                            if (180< hue && hue < 290 && value>0.1) //DAR O TUNE AQUI if (150 < hue && hue < 320) do upper green ao pink 
                            {
                                histogramY[y] = histogramY[y] + 1;
                                histogramX[x] = histogramX[x] + 1;
                                mapaBinarização[x, y] = 1;
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
                    var deltaX = Math.Abs(maxX - minX);
                    var deltaY = Math.Abs(maxY - minY);
                    int cantoInferiorX=0;
                    int cantoSuperiorX=0;
                    if (Math.Abs(deltaX- deltaY) > 10) //o tabuleiro está rodado
                    {
                        //ir buscar as coordenadas através do y min e y max, recorrendo ao mapaBinarização
                        for(x = 0; x < width; x++)
                        {
                            if(mapaBinarização[x, minY] == 1)
                            {
                                cantoInferiorX = x;
                            }
                            if (mapaBinarização[x, maxY] == 1)
                            {
                                cantoSuperiorX = x;
                            }
                        }
                        double angle = (Math.Atan((double)Math.Abs(maxY - minY) / (double)Math.Abs(cantoSuperiorX - cantoInferiorX)))* (180 / Math.PI);
                        angleRodar = angle - 45;
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
        public double FindBoardAngle(Image<Bgr, Byte> boardImg)
        {
            var imgCopy = boardImg.Copy();
            int minX = -1, maxX = -1, minY = -1, maxY = -1;
            double angleRodar = 0;
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

                int[,] mapaBinarização = new int[width, height]; //vamos encher este mapa com 0s e 1s
                                                                 // Preencher o array com zeros
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        mapaBinarização[i, j] = 0;
                    }
                }
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
                            if (180 < hue && hue < 230 && value > 0.5) //DAR O TUNE AQUI if (150 < hue && hue < 320) do upper green ao pink 
                            {
                                histogramY[y] = histogramY[y] + 1;
                                histogramX[x] = histogramX[x] + 1;
                                mapaBinarização[x, y] = 1;
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
                    var deltaX = Math.Abs(maxX - minX);
                    var deltaY = Math.Abs(maxY - minY);
                    int cantoInferiorX = 0;
                    int cantoSuperiorX = 0;
                    if (Math.Abs(deltaX - deltaY) > 10) //o tabuleiro está rodado
                    {
                        //ir buscar as coordenadas através do y min e y max, recorrendo ao mapaBinarização
                        for (x = 0; x < width; x++)
                        {
                            if (mapaBinarização[x, minY] == 1)
                            {
                                cantoInferiorX = x;
                            }
                            if (mapaBinarização[x, maxY] == 1)
                            {
                                cantoSuperiorX = x;
                            }
                        }
                        double angle = (Math.Atan((double)Math.Abs(maxY - minY) / (double)Math.Abs(cantoSuperiorX - cantoInferiorX))) * (180 / Math.PI);
                        angleRodar = angle - 45;
                    }



                }

            }

            //primeira coordenada ponta inferior esquerda 
            //segunda coordenada ponta superior direita
            return angleRodar * (float)Math.PI / 180.0f; 
        }
        public Coord[] FindPieceCoord(Image<Bgr, Byte> imgCopy)
        {
            int minX = -1, maxX = -1, minY = -1, maxY = -1;
            double percentagemPixeisPretos = 0;

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
                int numeroPixeisPretos = 0;
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
                                numeroPixeisPretos++;
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
                    percentagemPixeisPretos = ((float)numeroPixeisPretos / (width * height))*100;
                }
            }


            return percentagemPixeisPretos < 1 ?
                null :
                new Coord[2]
                {
                    new Coord(minX,maxX),
                    new Coord(minY,maxY),
                };
        }

        public IEnumerable<Image<Bgr, Byte>> CropHouseFromBoard(Image<Bgr, Byte> croppedBoard)
        {
            var width = croppedBoard.MIplImage.width;
            var height = croppedBoard.MIplImage.height;
            var pieces = new List<Image<Bgr, Byte>>();
            var pieceWidth = width / 8;
            var pieceHeight = height / 8;

            var letter = "A";
            var number = 1;

            for (int y = 0; y < height; y += pieceHeight)
            {
                letter += 1;
                for (int x = 0; x < width; x += pieceWidth)
                {
                    number++;
                    if (x + pieceWidth > width) break;
                    var imagemCortadinha = croppedBoard.GetSubRect(new Rectangle(x, y, pieceWidth, pieceHeight));
                    pieces.Add(imagemCortadinha);

                    string relativePath = Path.Combine("..", "..", $"ImagensCortadinhas/{letter}{number}.png");
                    string absolutePath = Path.GetFullPath(relativePath);
                    imagemCortadinha.Bitmap.Save(absolutePath, ImageFormat.Png);
                }

                if (y + pieceHeight > height) break;
            }

            return pieces;
        }

        public Image<Bgr, Byte> CropFigure(Image<Bgr, Byte> houseImg)
        {
            ImageClass.BinarizeImageWithColorToHsvBlack(houseImg);
            //[0] x min e x max
            //[1] y min e y max
            var coords = this.FindPieceCoord(houseImg.Copy());

            if (coords == null) return null;

            int width = Math.Abs(coords[0].X - coords[0].Y);
            int height = Math.Abs(coords[1].X - coords[1].Y);

            var croppedImg = houseImg.GetSubRect(new Rectangle(coords[0].X, coords[1].X, width+1, height+1));
            return croppedImg;
        }




        public Image<Bgr, Byte> CropBoard(Image<Bgr, Byte> boardImg, out string coordSuperior, out string coordInferior)
        {
            var boardCopy = boardImg.Copy();
            var angle = FindBoardAngle(boardCopy);
            if (angle > 3 * (float)Math.PI / 180.0f)
            {
                ImageClass.Rotation_BilinearParaBranco(boardCopy, boardCopy.Copy(), (float) angle);
                var relativePath = Path.Combine("..", "..", "dizerTipoPeca/tabuleirorodado.png");
                var absolutePath = Path.GetFullPath(relativePath);
                boardCopy.Bitmap.Save(absolutePath, ImageFormat.Png);
            }
            var boardCopyToWorkOn = boardCopy.Copy();
            var boardCopyToSubRect = boardCopy.Copy();

            // Encontra as coordenadas do tabuleiro na imagem
            var boardCoords = this.FindBoardCoords(boardCopyToWorkOn);

            // Calcula a largura do tabuleiro como a diferença absoluta entre as coordenadas X dos dois pontos opostos
            int boardWidth = Math.Abs(boardCoords[0].X - boardCoords[1].X);

            // Calcula a altura do tabuleiro como a diferença absoluta entre as coordenadas Y dos dois pontos opostos
            int boardHeight = Math.Abs(boardCoords[0].Y - boardCoords[1].Y);

            coordSuperior = $"({boardCoords[1].X},{boardCoords[1].Y})";
            coordInferior = $"({boardCoords[0].X},{boardCoords[0].Y})";

            // Retorna a subimagem do tabuleiro recortado
            return boardCopyToSubRect.GetSubRect(new Rectangle(boardCoords[0].X, boardCoords[1].Y, boardWidth, boardHeight));
        }

        public Image<Bgr, Byte> CropBoardHouse(Image<Bgr, Byte> croppedBoard, string boardHouse)
        {
            // Obtém a largura do tabuleiro recortado
            var boardWidth = croppedBoard.MIplImage.width;

            // Obtém a altura do tabuleiro recortado
            var boardHeight = croppedBoard.MIplImage.height;

            // Calcula a largura de uma casa do tabuleiro
            var houseWidth = boardWidth / 8;

            // Calcula a altura de uma casa do tabuleiro
            var houseHeight = boardHeight / 8;

            // Inicializa a letra e o número para identificar as casas do tabuleiro
            var letter = "A";
            var number = 1;

            // Percorre as linhas do tabuleiro
            for (int y = 0; y < boardHeight; y += houseHeight)
            {
                // Percorre as colunas do tabuleiro
                for (int x = 0; x < boardWidth; x += houseWidth)
                {
                    // Se a casa exceder a largura do tabuleiro, interrompe o loop
                    if (x + houseWidth > boardWidth) break;

                    // Verifica se a casa atual corresponde à casa especificada (boardHouse)
                    if (boardHouse.Equals($"{letter}{number}"))
                        // Retorna a subimagem correspondente à casa encontrada
                        return croppedBoard.GetSubRect(new Rectangle(x + (int)(houseWidth * 0.05), y +(int)(houseHeight * 0.05), houseWidth - (int)(houseWidth * 0.05), houseHeight - (int)(houseHeight * 0.05)));

                    // Incrementa o número para a próxima coluna
                    number++;
                }

                // Incrementa a letra para a próxima linha e reiniciar 
                letter = ((char)(letter[0] + 1)).ToString();
                number = 1;

                // Se a casa exceder a altura do tabuleiro, interrompe o loop
                if (y + houseHeight > boardHeight) break;
            }

            // Retorna null se a casa especificada não for encontrada
            return null;
        }



    }
}
