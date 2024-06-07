using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using ZedGraph;
using System.Diagnostics.Contracts;
using static Emgu.CV.StereoSGBM;
using System.Linq;
using System.Drawing;
using CG_OpenCV.Services;
using Emgu.CV.Flann;
using System.Drawing.Drawing2D;
using System.IO;
using Emgu.CV.CvEnum;
using System.Drawing.Imaging;

namespace CG_OpenCV
{
    class ImageClass
    {

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        /// Aula 2 Negativo

        public static void Negative(Image<Bgr, byte> img)
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

                            (dataPtr + x * nChan + y * step)[0] = (byte)(255 - blue);
                            (dataPtr + x * nChan + y * step)[1] = (byte)(255 - green);
                            (dataPtr + x * nChan + y * step)[2] = (byte)(255 - red);
                        }
                    }
                }
            }
        }
        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int step = m.widthStep;
                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = (byte)(int)Math.Round((dataPtr + nChan * x + step * y)[0] * contrast + bright);
                            green = (byte)(int)Math.Round((dataPtr + nChan * x + step * y)[1] * contrast + bright);
                            red = (byte)(int)Math.Round((dataPtr + nChan * x + step * y)[2] * contrast + bright);

                            if (blue >= 255)
                                blue = 255;
                            if (green >= 255)
                                green = 255;
                            if (red >= 255)
                                red = 255;
                            if (blue < 0)
                                blue = 0;
                            if (green < 0)
                                green = 0;
                            if (red < 0)
                                red = 0;

                            (dataPtr + x * nChan + y * step)[0] = (blue);
                            (dataPtr + x * nChan + y * step)[1] = (green);
                            (dataPtr + x * nChan + y * step)[2] = (red);
                        }
                    }
                }
            }
        }

        private static int pixel_modificado(int x, int y)
        {
            throw new NotImplementedException();
        }

        public static void RedChannel(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int step = m.widthStep;
                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components

                            blue = (dataPtr + x * nChan + y * step)[0];
                            green = (dataPtr + x * nChan + y * step)[1];
                            red = (dataPtr + x * nChan + y * step)[2];

                            (dataPtr + x * nChan + y * step)[0] = red;
                            (dataPtr + x * nChan + y * step)[1] = red;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }
        /*Implemente uma função que efetue a translação da imagem de um 
            deslocamento (Dx, Dy) introduzido pelo utilizador. 
            Este desvio poderá ser 
            positivo ou negativo caso se pretenda um deslocamento no sentido inverso.
        */
        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mC = imgCopy.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image 
                byte* dataPtrC = (byte*)mC.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red; // Espetro de cores

                int steps = m.widthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;

                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {

                        int newX, newY;

                        newX = x - dx;
                        newY = y - dy;

                        if (newX < width && newX >= 0 && newY < height && newY >= 0)
                        {
                            blue = (dataPtrC + newX * nChan + steps * newY)[0];
                            green = (dataPtrC + newX * nChan + steps * newY)[1];
                            red = (dataPtrC + newX * nChan + steps * newY)[2];
                        }
                        else
                        {
                            red = blue = green = 0;
                        }
                        (dataPtr + nChan * x + steps * y)[0] = blue;
                        (dataPtr + nChan * x + steps * y)[1] = green;
                        (dataPtr + nChan * x + steps * y)[2] = red;
                    }
                }
            }

        }
        // Função que vai efetuar a rotação da imagem
        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image


                // byte blue, green, red, gray;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int nChanCopy = mCopy.nChannels;
                //int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                int step = m.widthStep;
                int stepCopy = mCopy.widthStep;


                if (nChan == 3) // image in RGB
                {
                    int yO, xO = 0;

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            xO = (int)Math.Round((x - width / 2.0) * Math.Cos(angle) - (height / 2.0 - y) * Math.Sin(angle) + width / 2.0);

                            yO = (int)Math.Round(height / 2.0 - (x - width / 2.0) * Math.Sin(angle) - (height / 2.0 - y) * Math.Cos(angle));



                            if (xO >= 0 && xO < width && yO >= 0 && yO < height)
                            {

                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * xO + stepCopy * yO)[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * xO + stepCopy * yO)[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * xO + stepCopy * yO)[2];
                            }
                            else
                            {
                                (dataPtr + nChan * x + step * y)[0] = 0;
                                (dataPtr + nChan * x + step * y)[1] = 0;
                                (dataPtr + nChan * x + step * y)[2] = 0;
                            }

                        }

                    }
                }
            }
        }
        // Implemente uma função que efetue o zoom da imagem de um fator escala (S) introduzido pelo utilizador.
        // Este fator de escala deverá ser >1 para aumentar a imagem e <1 para reduzir a dimensão da imagem
        // Usar a operação de escalamento
        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mC = imgCopy.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image 
                byte* dataPtrC = (byte*)mC.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red;

                int steps = m.widthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        int newX, newY;
                        int centroX, centroY;

                        centroX = x - (width / 2);
                        centroY = (height / 2) - y;

                        newX = (int)Math.Round(x / scaleFactor);
                        newY = (int)Math.Round(y / scaleFactor);

                        if (newX < width && newX >= 0 && newY < height && newY >= 0)
                        {
                            blue = (dataPtrC + newX * nChan + steps * newY)[0];
                            green = (dataPtrC + newX * nChan + steps * newY)[1];
                            red = (dataPtrC + newX * nChan + steps * newY)[2];
                        }
                        else
                        {
                            red = blue = green = 0;
                        }
                        (dataPtr + nChan * x + steps * y)[0] = blue;
                        (dataPtr + nChan * x + steps * y)[1] = green;
                        (dataPtr + nChan * x + steps * y)[2] = red;

                    }
                }
            }
        }
        //Para melhorar a eficácia desta operação escolha com o rato o ponto de
        //referência(x0, y0) onde aplicar a operação de escalamento.
        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer of the image
                MIplImage mCopy = imgCopy.MIplImage;
                byte* ptrCopy = (byte*)mCopy.imageData.ToPointer();
                int step = m.widthStep;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - nChan * width; // bytes de alinhamento
                int x, y;

                if (nChan == 3) // Image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            int xOrigem = (int)Math.Round((x - width / 2) / scaleFactor + centerX);
                            int yOrigem = (int)Math.Round((y - height / 2) / scaleFactor + centerY);

                            byte blue, green, red;
                            if (xOrigem >= 0 && xOrigem < width && yOrigem >= 0 && yOrigem < height)
                            {
                                byte* ptr = ptrCopy + nChan * xOrigem + step * yOrigem;
                                blue = ptr[0];
                                green = ptr[1];
                                red = ptr[2];
                            }
                            else
                            {
                                blue = green = red = 0;
                            }

                            byte* ptrData = dataPtr + nChan * x + step * y;
                            ptrData[0] = blue;
                            ptrData[1] = green;
                            ptrData[2] = red;
                        }
                    }
                }
            }
        }
        //Aula 4
        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image

                byte blue, green, red, gray;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int nChanCopy = mCopy.nChannels;
                //  int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;


                int step = m.widthStep;
                int stepCopy = mCopy.widthStep;


                if (nChan == 3) // image in RGB
                {

                    for (y = 1; y < height - 1; y++)
                    {
                        for (x = 1; x < width - 1; x++)
                        {
                            (dataPtr + nChan * x + step * y)[0] = (byte)Math.Round((
                                                      ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0]) +
                                                      ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 1))[0]) +
                                                      ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0]) +
                                                      ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 0))[0]) +
                                                      ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 0))[0]) +
                                                      ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 0))[0]) +
                                                      ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0]) +
                                                      ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y + 1))[0]) +
                                                      ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0]))

                                                   / 9.0);



                            (dataPtr + nChan * x + step * y)[1] = (byte)Math.Round((

                                    ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1]) +
                                    ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 1))[1]) +
                                    ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1]) +
                                    ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 0))[1]) +
                                    ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 0))[1]) +
                                    ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 0))[1]) +
                                    ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1]) +
                                    ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y + 1))[1]) +
                                    ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1]))

                                    / 9.0); ;


                            (dataPtr + nChan * x + step * y)[2] = (byte)Math.Round((

                                    ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2]) +
                                    ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 1))[2]) +
                                    ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2]) +
                                    ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2]) +
                                    ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * y)[2]) +
                                    ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2]) +
                                    ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2]) +
                                    ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y + 1))[2]) +
                                    ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2]))

                                    / 9.0);


                            //  (dataPtr + nChan * x + step * y)[0] = blue;
                            // (dataPtr + nChan * x + step * y)[1] = green;
                            // (dataPtr + nChan * x + step * y)[2] = red;
                        }
                    }

                    // LINHAS HORIZONTAIS

                    // CIMA  

                    for (x = 1; x < width - 1; x++)
                    {
                        (dataPtr + nChan * x + step * 0)[0] = (byte)Math.Round((

                                                                                   2 * (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[0] +
                                                                                   2 * (dataPtrCopy + nChanCopy * x + stepCopy * 0)[0] +
                                                                                   2 * (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[0] +
                                                                                   ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (0 + 1))[0]) +
                                                                                   ((dataPtrCopy + nChanCopy * x + stepCopy * (0 + 1))[0]) +
                                                                                   ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (0 + 1))[0]))

                                                                               / 9.0);

                        (dataPtr + nChan * x + step * 0)[1] = (byte)Math.Round((

                                                                                   2 * (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[1] +
                                                                                   2 * (dataPtrCopy + nChanCopy * x + stepCopy * 0)[1] +
                                                                                   2 * (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[1] +
                                                                                   ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (0 + 1))[1]) +
                                                                                   ((dataPtrCopy + nChanCopy * x + stepCopy * (0 + 1))[1]) +
                                                                                   ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (0 + 1))[1]))

                                                                               / 9.0);

                        (dataPtr + nChan * x + step * 0)[2] = (byte)Math.Round((

                                                                                   2 * (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[2] +
                                                                                   2 * (dataPtrCopy + nChanCopy * x + stepCopy * 0)[2] +
                                                                                   2 * (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[2] +
                                                                                   ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 1)[2]) +
                                                                                   ((dataPtrCopy + nChanCopy * x + stepCopy * 1)[2]) +
                                                                                   ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 1)[2]))

                                                                               / 9.0);

                        // (dataPtr + nChan * x + step * 0)[0] = blue;
                        // (dataPtr + nChan * x + step * 0)[1] = green;
                        // (dataPtr + nChan * x + step * 0)[2] = red;


                        //------------------------------------------

                        //BAIXO

                        (dataPtr + nChan * x + step * (height - 1))[0] = (byte)Math.Round((

                                    2 * (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[0] +
                                    2 * (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[0] +
                                    2 * (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[0] +
                                    ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[0]) +
                                    ((dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[0]) +
                                    ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[0]))

                                    / 9.0);


                        (dataPtr + nChan * x + step * (height - 1))[1] = (byte)Math.Round((

                                 2 * (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[1] +
                                 2 * (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[1] +
                                 2 * (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[1] +
                                 ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[1]) +
                                 ((dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[1]) +
                                 ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[1]))

                             / 9.0);

                        (dataPtr + nChan * x + step * (height - 1))[2] = (byte)Math.Round((

                                2 * (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[2] +
                                2 * (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[2] +
                                2 * (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[2] +
                                ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[2]) +
                                ((dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[2]) +
                                ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[2]))

                            / 9.0);

                    }


                    //LINHAS VERTICAIS


                    // ESQUERDA

                    for (y = 1; y < height - 1; y++)
                    {
                        (dataPtr + nChan * 0 + step * y)[0] = (byte)Math.Round((

                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[0] +
                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[0] +
                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[0] +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[0]) +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * y)[0]) +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[0]))

                                                                               / 9.0);

                        (dataPtr + nChan * 0 + step * y)[1] = (byte)Math.Round((

                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[1] +
                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[1] +
                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[1] +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[1]) +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * y)[1]) +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[1]))

                                                                               / 9.0);

                        (dataPtr + nChan * 0 + step * y)[2] = (byte)Math.Round((

                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[2] +
                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[2] +
                                                                                   2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[2] +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[2]) +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * y)[2]) +
                                                                                   ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[2]))

                                                                               / 9.0);




                        // DIREITA

                        //--------------


                        (dataPtr + nChan * (width - 1) + step * y)[0] = (byte)Math.Round((

                                   2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[0] +
                                   2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[0] +
                                   2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[0] +

                                   (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[0] +
                                   ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[0]) +
                                   ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[0]))

                                   / 9.0);


                        (dataPtr + nChan * (width - 1) + step * y)[1] = (byte)Math.Round((

                                2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[1] +
                                2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[1] +
                                2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[1] +

                                  (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[1] +
                                  ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[1]) +
                                  ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[1]))

                             / 9.0);

                        (dataPtr + nChan * (width - 1) + step * y)[2] = (byte)Math.Round((

                                2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[2] +
                                2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[2] +
                                2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[2] +

                                (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[2] +
                                ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[2]) +
                                ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[2]))

                                / 9.0);


                    }


                    //CANTOS

                    // ponto 0 : 0


                    (dataPtr + nChan * 0 + step * 0)[0] = (byte)Math.Round((
                        4 * (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[0] +
                        2 * (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[0] +
                        2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[0] +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[0]) / 9.0);


                    (dataPtr + nChan * 0 + step * 0)[1] = (byte)Math.Round((
                        4 * (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[1] +
                        2 * (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[1] +
                        2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[1] +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[1]) / 9.0);


                    (dataPtr + nChan * 0 + step * 0)[2] = (byte)Math.Round((
                        4 * (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[2] +
                        2 * (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[2] +
                        2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[2] +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[2]) / 9.0);


                    // ponto width -1 : 0


                    (dataPtr + nChan * (width - 1) + step * 0)[0] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[0] +
                        2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[0] +
                        2 * (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[0] +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[0]) / 9.0);


                    (dataPtr + nChan * (width - 1) + step * 0)[1] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[1] +
                        2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[1] +
                        2 * (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[1] +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[1]) / 9.0);


                    (dataPtr + nChan * (width - 1) + step * 0)[2] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[2] +
                        2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[2] +
                        2 * (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[2] +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[2]) / 9.0);


                    // ponto 0 : height - 1


                    (dataPtr + nChan * 0 + step * (height - 1))[0] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[0] +
                        2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[0] +
                        2 * (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[0] +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[0]) / 9.0);

                    (dataPtr + nChan * 0 + step * (height - 1))[1] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[1] +
                        2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[1] +
                        2 * (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[1] +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[1]) / 9.0);

                    (dataPtr + nChan * 0 + step * (height - 1))[2] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[2] +
                        2 * (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[2] +
                        2 * (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[2] +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[2]) / 9.0);




                    //ponto width -1  : height -1


                    (dataPtr + nChan * (width - 1) + step * (height - 1))[0] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[0] +
                        2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[0] +
                        2 * (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[0] +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[0]) / 9.0);

                    (dataPtr + nChan * (width - 1) + step * (height - 1))[1] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[1] +
                        2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[1] +
                        2 * (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[1] +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[1]) / 9.0);

                    (dataPtr + nChan * (width - 1) + step * (height - 1))[2] = (byte)Math.Round((

                        4 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[2] +
                        2 * (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[2] +
                        2 * (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[2] +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[2]) / 9.0);


                }
            }
        }

        //Aula 5
        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix,
    float matrixWeight)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image


                // byte blue, green, red, gray;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int nChanCopy = mCopy.nChannels;
                //  int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;


                int step = m.widthStep;
                int stepCopy = mCopy.widthStep;


                if (nChan == 3) // image in RGB
                {

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // CORE
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    for (y = 1; y < height - 1; y++)
                    {
                        for (x = 1; x < width - 1; x++)
                        {
                            int blueAux = (int)Math.Round((

                                                      ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0]) * matrix[0, 0] +
                                                      ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 1))[0]) * matrix[0, 1] +
                                                      ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0]) * matrix[0, 2] +
                                                      ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 0))[0]) * matrix[1, 0] +
                                                      ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 0))[0]) * matrix[1, 1] +
                                                      ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 0))[0]) * matrix[1, 2] +
                                                      ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0]) * matrix[2, 0] +
                                                      ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y + 1))[0]) * matrix[2, 1] +
                                                      ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0]) * matrix[2, 2])

                                                   / matrixWeight);

                            if (blueAux < 0)
                            {
                                (dataPtr + nChan * x + step * y)[0] = 0;
                            }
                            else if (blueAux > 255)
                            {
                                (dataPtr + nChan * x + step * y)[0] = 255;
                            }
                            else
                            {
                                (dataPtr + nChan * x + step * y)[0] = (byte)blueAux;
                            }



                            int greenAux = (int)Math.Round((
                                                         ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1]) * matrix[0, 0] +
                                                         ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 1))[1]) * matrix[0, 1] +
                                                         ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1]) * matrix[0, 2] +
                                                         ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 0))[1]) * matrix[1, 0] +
                                                         ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 0))[1]) * matrix[1, 1] +
                                                         ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 0))[1]) * matrix[1, 2] +
                                                         ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1]) * matrix[2, 0] +
                                                         ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y + 1))[1]) * matrix[2, 1] +
                                                         ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1]) * matrix[2, 2])

                                / matrixWeight);

                            if (greenAux < 0)
                            {
                                (dataPtr + nChan * x + step * y)[1] = 0;
                            }
                            else if (greenAux > 255)
                            {
                                (dataPtr + nChan * x + step * y)[1] = 255;
                            }
                            else
                            {
                                (dataPtr + nChan * x + step * y)[1] = (byte)greenAux;
                            }



                            int redAux = (int)Math.Round((

                                                          ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2]) * matrix[0, 0] +
                                                          ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 1))[2]) * matrix[0, 1] +
                                                          ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2]) * matrix[0, 2] +
                                                          ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 0))[2]) * matrix[1, 0] +
                                                          ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y - 0))[2]) * matrix[1, 1] +
                                                          ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 0))[2]) * matrix[1, 2] +
                                                          ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2]) * matrix[2, 0] +
                                                          ((dataPtrCopy + nChanCopy * (x - 0) + stepCopy * (y + 1))[2]) * matrix[2, 1] +
                                                          ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2]) * matrix[2, 2])
                                / matrixWeight);


                            if (redAux < 0)
                            {
                                (dataPtr + nChan * x + step * y)[2] = 0;
                            }
                            else if (redAux > 255)
                            {
                                (dataPtr + nChan * x + step * y)[2] = 255;
                            }
                            else
                            {
                                (dataPtr + nChan * x + step * y)[2] = (byte)redAux;
                            }


                        }
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // BORDERS
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    // LINHAS HORIZONTAIS
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // UP  

                    for (x = 1; x < width - 1; x++)
                    {
                        int blueAux = (int)Math.Round((

                                                       (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[0] * (matrix[0, 0] + matrix[1, 0]) +
                                                       (dataPtrCopy + nChanCopy * x + stepCopy * 0)[0] * (matrix[0, 1] + matrix[1, 1]) +
                                                       (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[0] * (matrix[0, 2] + matrix[1, 2]) +

                                                       ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 1)[0]) * matrix[2, 0] +
                                                       ((dataPtrCopy + nChanCopy * x + stepCopy * 1)[0]) * matrix[2, 1] +
                                                       ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 1)[0]) * matrix[2, 2])

                                                   / matrixWeight);

                        if (blueAux < 0)
                        {
                            (dataPtr + nChan * x + step * 0)[0] = 0;
                        }
                        else if (blueAux > 255)
                        {
                            (dataPtr + nChan * x + step * 0)[0] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * x + step * 0)[0] = (byte)blueAux;
                        }


                        int greenAux = (int)Math.Round((
                                                        (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[1] * (matrix[0, 0] + matrix[1, 0]) +
                                                       (dataPtrCopy + nChanCopy * x + stepCopy * 0)[1] * (matrix[0, 1] + matrix[1, 1]) +
                                                       (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[1] * (matrix[0, 2] + matrix[1, 2]) +

                                                       ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 1)[1]) * matrix[2, 0] +
                                                       ((dataPtrCopy + nChanCopy * x + stepCopy * 1)[1]) * matrix[2, 1] +
                                                       ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 1)[1]) * matrix[2, 2])

                                                   / matrixWeight);

                        if (greenAux < 0)
                        {
                            (dataPtr + nChan * x + step * 0)[1] = 0;
                        }
                        else if (greenAux > 255)
                        {
                            (dataPtr + nChan * x + step * 0)[1] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * x + step * 0)[1] = (byte)greenAux;
                        }

                        int redAux = (int)Math.Round((

                                                     (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[2] * (matrix[0, 0] + matrix[1, 0]) +
                                                       (dataPtrCopy + nChanCopy * x + stepCopy * 0)[2] * (matrix[0, 1] + matrix[1, 1]) +
                                                       (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[2] * (matrix[0, 2] + matrix[1, 2]) +

                                                       ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 1)[2]) * matrix[2, 0] +
                                                       ((dataPtrCopy + nChanCopy * x + stepCopy * 1)[2]) * matrix[2, 1] +
                                                       ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 1)[2]) * matrix[2, 2])

                                                   / matrixWeight);

                        if (redAux < 0)
                        {
                            (dataPtr + nChan * x + step * 0)[2] = 0;
                        }
                        else if (redAux > 255)
                        {
                            (dataPtr + nChan * x + step * 0)[2] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * x + step * 0)[2] = (byte)redAux;
                        }



                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //DOWN

                        blueAux = (int)Math.Round((
                                                (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[0] * (matrix[2, 0] + matrix[1, 0]) +
                                                // (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[0] * matrix[2, 0] +
                                                (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[0] * (matrix[2, 1] + matrix[1, 1]) +
                                                //(dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[0] * matrix[2, 1] +
                                                (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[0] * (matrix[2, 2] + matrix[2, 1]) +
                                                // (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[0] * matrix[2, 2] +
                                                ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[0]) * matrix[0, 0] +
                                                ((dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[0]) * matrix[0, 1] +
                                                ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[0]) * matrix[0, 2])

                                   / matrixWeight);

                        if (blueAux < 0)
                        {
                            (dataPtr + nChan * x + step * (height - 1))[0] = 0;
                        }

                        else if (blueAux > 255)
                        {
                            (dataPtr + nChan * x + step * (height - 1))[0] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * x + step * (height - 1))[0] = (byte)blueAux;
                        }


                        greenAux = (int)Math.Round((

                                                    (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[1] * (matrix[2, 0] + matrix[1, 0]) +
                                                // (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[0] * matrix[2, 0] +
                                                (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[1] * (matrix[2, 1] + matrix[1, 1]) +
                                                //(dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[0] * matrix[2, 1] +
                                                (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[1] * (matrix[2, 2] + matrix[2, 1]) +
                                                // (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[0] * matrix[2, 2] +
                                                ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[1]) * matrix[0, 0] +
                                                ((dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[1]) * matrix[0, 1] +
                                                ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[1]) * matrix[0, 2])

                                               / matrixWeight);

                        if (greenAux < 0)
                        {
                            (dataPtr + nChan * x + step * (height - 1))[1] = 0;
                        }

                        else if (greenAux > 255)
                        {
                            (dataPtr + nChan * x + step * (height - 1))[1] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * x + step * (height - 1))[1] = (byte)greenAux;
                        }




                        redAux = (int)Math.Round((

                                                 (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[2] * (matrix[2, 0] + matrix[1, 0]) +
                                                // (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[0] * matrix[2, 0] +
                                                (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[2] * (matrix[2, 1] + matrix[1, 1]) +
                                                //(dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[0] * matrix[2, 1] +
                                                (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[2] * (matrix[2, 2] + matrix[2, 1]) +
                                                // (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[0] * matrix[2, 2] +
                                                ((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[2]) * matrix[0, 0] +
                                                ((dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[2]) * matrix[0, 1] +
                                                ((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[2]) * matrix[0, 2])

                                                / matrixWeight);

                        if (redAux < 0)
                        {
                            (dataPtr + nChan * x + step * (height - 1))[2] = 0;
                        }

                        else if (redAux > 255)
                        {
                            (dataPtr + nChan * x + step * (height - 1))[2] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * x + step * (height - 1))[2] = (byte)redAux;
                        }

                    }


                    //LINHAS VERTICAIS
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // ESQUERDA

                    for (y = 1; y < height - 1; y++)
                    {
                        int blueAux = (int)Math.Round(((dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[0] * (matrix[0, 0] + matrix[0, 1]) +
                                                    (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[0] * (matrix[1, 0] + matrix[1, 1]) +
                                                    (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[0] * (matrix[2, 0] + matrix[2, 1]) +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[0]) * matrix[0, 2] +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * y)[0]) * matrix[1, 2] +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[0] * matrix[2, 2]))

                                                                               / matrixWeight);

                        if (blueAux < 0)
                        {
                            (dataPtr + nChan * 0 + step * y)[0] = 0;
                        }
                        else if (blueAux > 255)
                        {
                            (dataPtr + nChan * 0 + step * y)[0] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + step * y)[0] = (byte)blueAux;
                        }



                        int greenAux = (int)Math.Round((
                                                       (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[1] * (matrix[0, 0] + matrix[0, 1]) +
                                                    // (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[0] * matrix[0, 0] +
                                                    (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[1] * (matrix[1, 0] + matrix[1, 1]) +
                                                    // (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[0] * matrix[1, 0] +
                                                    (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[1] * (matrix[2, 0] + matrix[2, 1]) +
                                                    // (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[0] * matrix[2, 0] +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[1]) * matrix[0, 2] +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * y)[1]) * matrix[1, 2] +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[1] * matrix[2, 2]))

                                                   / matrixWeight);

                        if (greenAux < 0)
                        {
                            (dataPtr + nChan * 0 + step * y)[1] = 0;
                        }
                        else if (greenAux > 255)
                        {
                            (dataPtr + nChan * 0 + step * y)[1] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + step * y)[1] = (byte)greenAux;
                        }


                        int redAux = (int)Math.Round(
                                                      ((dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[2] * (matrix[0, 0] + matrix[0, 1]) +
                                                    // (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[0] * matrix[0, 0] +
                                                    (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[2] * (matrix[1, 0] + matrix[1, 1]) +
                                                    // (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[0] * matrix[1, 0] +
                                                    (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[2] * (matrix[2, 0] + matrix[2, 1]) +
                                                    // (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[0] * matrix[2, 0] +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[2]) * matrix[0, 2] +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * y)[2]) * matrix[1, 2] +
                                                    ((dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[2] * matrix[2, 2]))

                                                   / matrixWeight);

                        if (redAux < 0)
                        {
                            (dataPtr + nChan * 0 + step * y)[2] = 0;
                        }
                        else if (redAux > 255)
                        {
                            (dataPtr + nChan * 0 + step * y)[2] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + step * y)[2] = (byte)redAux;
                        }




                        // DIREITA

                        //--------------


                        blueAux = (int)Math.Round((
                               (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[0] * (matrix[0, 1] + matrix[0, 2]) +
                               //   (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[0] * matrix[0, 1] +
                               (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[0] * (matrix[1, 1] + matrix[1, 2]) +
                               //   (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[0] * matrix[1, 1] +
                               (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[0] * (matrix[2, 1] + matrix[2, 2]) +
                               //     (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[0] * matrix[2, 1] +
                               (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[0] * matrix[0, 0] +
                               ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[0]) * matrix[1, 0] +
                               (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[0] * matrix[2, 0])

                                  / matrixWeight);

                        if (blueAux < 0)
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[0] = 0;
                        }
                        else if (blueAux > 255)
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[0] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (byte)blueAux;
                        }


                        greenAux = (int)Math.Round((
                               (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[1] * (matrix[0, 1] + matrix[0, 2]) +
                               //   (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[0] * matrix[0, 1] +
                               (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[1] * (matrix[1, 1] + matrix[1, 2]) +
                               //   (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[0] * matrix[1, 1] +
                               (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[1] * (matrix[2, 1] + matrix[2, 2]) +
                               //     (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[0] * matrix[2, 1] +
                               (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[1] * matrix[0, 0] +
                               ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[1]) * matrix[1, 0] +
                               (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[1] * matrix[2, 0])

                                               / matrixWeight);

                        if (greenAux < 0)
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[1] = 0;
                        }
                        else if (greenAux > 255)
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[1] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (byte)greenAux;
                        }

                        redAux = (int)Math.Round((

                       (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[2] * (matrix[0, 1] + matrix[0, 2]) +
                               //   (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[0] * matrix[0, 1] +
                               (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[2] * (matrix[1, 1] + matrix[1, 2]) +
                               //   (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[0] * matrix[1, 1] +
                               (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[2] * (matrix[2, 1] + matrix[2, 2]) +
                               //     (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[0] * matrix[2, 1] +
                               (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[2] * matrix[0, 0] +
                               ((dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[2]) * matrix[1, 0] +
                               (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[2] * matrix[2, 0])

                                               / matrixWeight);

                        if (redAux < 0)
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[2] = 0;
                        }
                        else if (redAux > 255)
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[2] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (byte)redAux;
                        }


                    }


                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //CANTOS
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // UP LEFT
                    {
                        int blueAux = (int)Math.Round((
                            (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[0] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1]) +
                            (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[0] * (matrix[0, 2] + matrix[1, 2]) +
                            (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[0] * (matrix[2, 0] + matrix[2, 1]) +
                            (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[0] * matrix[2, 2]) / matrixWeight);

                        if (blueAux < 0)
                        {
                            (dataPtr + nChan * 0 + step * 0)[0] = 0;
                        }

                        else if (blueAux > 255)
                        {
                            (dataPtr + nChan * 0 + step * 0)[0] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + step * 0)[0] = (byte)blueAux;
                        }

                        int greenAux = (int)Math.Round((
                            (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[1] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1]) +
                            (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[1] * (matrix[0, 2] + matrix[1, 2]) +
                            (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[1] * (matrix[2, 0] + matrix[2, 1]) +
                            (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[1] * matrix[2, 2]) / matrixWeight);

                        if (greenAux < 0)
                        {
                            (dataPtr + nChan * 0 + step * 0)[1] = 0;
                        }

                        else if (greenAux > 255)
                        {
                            (dataPtr + nChan * 0 + step * 0)[1] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + step * 0)[1] = (byte)greenAux;
                        }


                        int redAux = (int)Math.Round((
                            (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[2] * (matrix[0, 0] + matrix[0, 1] + matrix[1, 0] + matrix[1, 1]) +
                            (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[2] * (matrix[0, 2] + matrix[1, 2]) +
                            (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[2] * (matrix[2, 0] + matrix[2, 1]) +
                            (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[2] * matrix[2, 2]) / matrixWeight);

                        if (redAux < 0)
                        {
                            (dataPtr + nChan * 0 + step * 0)[2] = 0;
                        }

                        else if (redAux > 255)
                        {
                            (dataPtr + nChan * 0 + step * 0)[2] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + step * 0)[2] = (byte)redAux;
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // UP RIGHT

                    int blue = (int)Math.Round((

                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[0] * (matrix[0, 2] + matrix[0, 1] + matrix[1, 2] + matrix[1, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[0] * (matrix[2, 2] + matrix[2, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[0] * (matrix[0, 0] + matrix[1, 0]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[0] * matrix[2, 0]) / matrixWeight);



                    if (blue < 0)
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[0] = 0;
                    }

                    else if (blue > 255)
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[0] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[0] = (byte)blue;
                    }




                    int green = (int)Math.Round((

                          (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[1] * (matrix[0, 2] + matrix[0, 1] + matrix[1, 2] + matrix[1, 1]) +
                          (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[1] * (matrix[2, 2] + matrix[2, 1]) +
                          (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[1] * (matrix[0, 0] + matrix[1, 0]) +
                          (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[1] * matrix[2, 0]) / matrixWeight);



                    if (green < 0)
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[1] = 0;
                    }

                    else if (green > 255)
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[1] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[1] = (byte)green;
                    }


                    int red = (int)Math.Round((

                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[2] * (matrix[0, 2] + matrix[0, 1] + matrix[1, 2] + matrix[1, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[2] * (matrix[2, 2] + matrix[2, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[2] * (matrix[0, 0] + matrix[1, 0]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[2] * matrix[2, 0]) / matrixWeight);



                    if (red < 0)
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[2] = 0;
                    }

                    else if (red > 255)
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[2] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + step * 0)[2] = (byte)red;
                    }


                    ///////////////////////////////////////////////////////////////////////////////////////////////
                    /// BOTTOM LEFT
                    /// 
                    blue = (byte)Math.Round((

                        (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[0] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) +
                        (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[0] * (matrix[0, 0] + matrix[0, 1]) +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[0] * (matrix[1, 2] + matrix[2, 2]) +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[0] * matrix[0, 2]) / matrixWeight);


                    if (blue < 0)
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[0] = 0;
                    }
                    else if (blue > 255)
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[0] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[0] = (byte)blue;
                    }

                    red = (byte)Math.Round((

                        (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[1] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) +
                        (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[1] * (matrix[0, 0] + matrix[0, 1]) +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[1] * (matrix[1, 2] + matrix[2, 2]) +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[1] * matrix[0, 2]) / matrixWeight);


                    if (red < 0)
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[1] = 0;
                    }
                    else if (red > 255)
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[1] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[1] = (byte)red;
                    }


                    green = (byte)Math.Round((

                        (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[2] * (matrix[1, 0] + matrix[1, 1] + matrix[2, 0] + matrix[2, 1]) +
                        (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[2] * (matrix[0, 0] + matrix[0, 1]) +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[2] * (matrix[1, 2] + matrix[2, 2]) +
                        (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[2] * matrix[0, 2]) / matrixWeight);


                    if (green < 0)
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[2] = 0;
                    }
                    else if (green > 255)
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[2] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + step * (height - 1))[2] = (byte)green;
                    }


                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // BOTTOM RIGHT

                    blue = (int)Math.Round((

                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[0] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 2] + matrix[2, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[0] * (matrix[0, 2] + matrix[0, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[0] * (matrix[1, 0] + matrix[2, 0]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[0] * (matrix[0, 0])) / matrixWeight);

                    if (blue < 0)
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[0] = 0;
                    }
                    else if (blue > 255)
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[0] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[0] = (byte)blue;
                    }

                    green = (int)Math.Round((

                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[1] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 2] + matrix[2, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[1] * (matrix[0, 2] + matrix[0, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[1] * (matrix[1, 0] + matrix[2, 0]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[1] * (matrix[0, 0])) / matrixWeight);

                    if (green < 0)
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[1] = 0;
                    }
                    else if (green > 255)
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[1] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[1] = (byte)green;
                    }

                    red = (int)Math.Round((

                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[2] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 2] + matrix[2, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[2] * (matrix[0, 2] + matrix[0, 1]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[2] * (matrix[1, 0] + matrix[2, 0]) +
                        (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[2] * (matrix[0, 0])) / matrixWeight);

                    if (red < 0)
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[2] = 0;
                    }
                    else if (red > 255)
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[2] = 255;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + step * (height - 1))[2] = (byte)red;
                    }

                }
            }
        }
        // Aula 6
        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int nChanCopy = mCopy.nChannels;
                int x, y;
                int a, b, c, d, e, f, g, h, i;
                int sX, sY;
                int blue, green, red;

                int step = m.widthStep;
                int stepCopy = mCopy.widthStep;


                if (nChan == 3) // image in RGB
                {

                    for (y = 1; y < height - 1; y++)
                    {
                        for (x = 1; x < width - 1; x++)
                        {
                            //BLUE
                            a = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0];
                            b = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            c = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0];
                            d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            e = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            g = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0];
                            h = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                            i = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0];

                            sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                            sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                            blue = sX + sY;

                            blue = (blue > 255) ? 255 : blue;
                            blue = (blue < 0) ? 0 : blue;

                            (dataPtr + nChan * x + step * y)[0] = (byte)blue;


                            //GREEN
                            a = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1];
                            b = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            c = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1];
                            d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            e = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            g = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1];
                            h = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                            i = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1];

                            sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                            sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                            green = sX + sY;

                            green = (green > 255) ? 255 : green;
                            green = (green < 0) ? 0 : green;

                            (dataPtr + nChan * x + step * y)[1] = (byte)green;


                            //RED
                            a = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2];
                            b = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            c = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2];
                            d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            e = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            g = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2];
                            h = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                            i = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2];

                            sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                            sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                            red = sX + sY;

                            red = (red > 255) ? 255 : red;
                            red = (red < 0) ? 0 : red;

                            (dataPtr + nChan * x + step * y)[2] = (byte)red;

                        }
                    }

                    // LINHAS HORIZONTAIS

                    // CIMA  

                    for (x = 1; x < width - 1; x++)
                    {
                        //BLUE
                        a = d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[0];
                        b = e = (dataPtrCopy + nChanCopy * x + stepCopy * 0)[0];
                        c = f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[0];
                        g = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 1)[0];
                        h = (dataPtrCopy + nChanCopy * x + stepCopy * 1)[0];
                        i = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 1)[0];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        blue = sX + sY;

                        blue = (blue > 255) ? 255 : blue;
                        blue = (blue < 0) ? 0 : blue;

                        (dataPtr + nChan * x + step * 0)[0] = (byte)blue;


                        //GREEN
                        a = d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[1];
                        b = e = (dataPtrCopy + nChanCopy * x + stepCopy * 0)[1];
                        c = f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[1];
                        g = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 1)[1];
                        h = (dataPtrCopy + nChanCopy * x + stepCopy * 1)[1];
                        i = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 1)[1];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        green = sX + sY;

                        green = (green > 255) ? 255 : green;
                        green = (green < 0) ? 0 : green;

                        (dataPtr + nChan * x + step * 0)[1] = (byte)green;


                        //RED
                        a = d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 0)[2];
                        b = e = (dataPtrCopy + nChanCopy * x + stepCopy * 0)[2];
                        c = f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 0)[2];
                        g = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * 1)[2];
                        h = (dataPtrCopy + nChanCopy * x + stepCopy * 1)[2];
                        i = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * 1)[2];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        red = sX + sY;

                        red = (red > 255) ? 255 : red;
                        red = (red < 0) ? 0 : red;

                        (dataPtr + nChan * x + step * 0)[2] = (byte)red;



                        //------------------------------------------

                        //BAIXO

                        //BLUE
                        g = d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[0];
                        h = e = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[0];
                        i = f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[0];
                        a = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[0];
                        b = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[0];
                        c = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[0];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        blue = sX + sY;

                        blue = (blue > 255) ? 255 : blue;
                        blue = (blue < 0) ? 0 : blue;

                        (dataPtr + nChan * x + step * (height - 1))[0] = (byte)blue;


                        //GREEN
                        g = d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[1];
                        h = e = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[1];
                        i = f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[1];
                        a = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[1];
                        b = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[1];
                        c = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[1];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        green = sX + sY;

                        green = (green > 255) ? 255 : green;
                        green = (green < 0) ? 0 : green;

                        (dataPtr + nChan * x + step * (height - 1))[1] = (byte)green;


                        //GREEN
                        g = d = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 1))[2];
                        h = e = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[2];
                        i = f = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[2];
                        a = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (height - 2))[2];
                        b = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 2))[2];
                        c = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 2))[2];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        red = sX + sY;

                        red = (red > 255) ? 255 : red;
                        red = (red < 0) ? 0 : red;

                        (dataPtr + nChan * x + step * (height - 1))[2] = (byte)red;
                    }

                    // VERTICAIS

                    for (y = 1; y < height - 1; y++)
                    {
                        //LEFT

                        //BLUE
                        a = b = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[0];
                        d = e = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[0];
                        g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[0];
                        c = (dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[0];
                        f = (dataPtrCopy + nChanCopy * 1 + stepCopy * y)[0];
                        i = (dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[0];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        blue = sX + sY;

                        blue = (blue > 255) ? 255 : blue;
                        blue = (blue < 0) ? 0 : blue;

                        (dataPtr + nChan * 0 + step * y)[0] = (byte)blue;


                        //GREEN
                        a = b = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[1];
                        d = e = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[1];
                        g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[1];
                        c = (dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[1];
                        f = (dataPtrCopy + nChanCopy * 1 + stepCopy * y)[1];
                        i = (dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[1];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        green = sX + sY;

                        green = (green > 255) ? 255 : green;
                        green = (green < 0) ? 0 : green;

                        (dataPtr + nChan * 0 + step * y)[1] = (byte)green;


                        //RED
                        a = b = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y - 1))[2];
                        d = e = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y))[2];
                        g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * (y + 1))[2];
                        c = (dataPtrCopy + nChanCopy * 1 + stepCopy * (y - 1))[2];
                        f = (dataPtrCopy + nChanCopy * 1 + stepCopy * y)[2];
                        i = (dataPtrCopy + nChanCopy * 1 + stepCopy * (y + 1))[2];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        red = sX + sY;

                        red = (red > 255) ? 255 : red;
                        red = (red < 0) ? 0 : red;

                        (dataPtr + nChan * 0 + step * y)[2] = (byte)red;


                        // DIREITA

                        //--------------
                        //BLUE
                        b = c = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[0];
                        e = f = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[0];
                        h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[0];
                        a = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[0];
                        d = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[0];
                        g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[0];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        blue = sX + sY;

                        blue = (blue > 255) ? 255 : blue;
                        blue = (blue < 0) ? 0 : blue;

                        (dataPtr + nChan * (width - 1) + step * y)[0] = (byte)blue;


                        //GREEN
                        b = c = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[1];
                        e = f = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[1];
                        h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[1];
                        a = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[1];
                        d = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[1];
                        g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[1];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        green = sX + sY;

                        green = (green > 255) ? 255 : green;
                        green = (green < 0) ? 0 : green;

                        (dataPtr + nChan * (width - 1) + step * y)[1] = (byte)green;


                        //RED
                        b = c = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y - 1))[2];
                        e = f = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[2];
                        h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[2];
                        a = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y - 1))[2];
                        d = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * y)[2];
                        g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (y + 1))[2];

                        sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                        sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                        red = sX + sY;

                        red = (red > 255) ? 255 : red;
                        red = (red < 0) ? 0 : red;

                        (dataPtr + nChan * (width - 1) + step * y)[2] = (byte)red;
                    }



                    //CANTOS

                    // UP LEFT 

                    //BLUE
                    a = b = d = e = (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[0];
                    c = f = (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[0];
                    g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[0];
                    i = (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[0];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    blue = sX + sY;

                    blue = (blue > 255) ? 255 : blue;
                    blue = (blue < 0) ? 0 : blue;

                    (dataPtr + nChan * 0 + step * 0)[0] = (byte)blue;


                    //GREEN
                    a = b = d = e = (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[1];
                    c = f = (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[1];
                    g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[1];
                    i = (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[1];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    green = sX + sY;

                    green = (green > 255) ? 255 : green;
                    green = (green < 0) ? 0 : green;

                    (dataPtr + nChan * 0 + step * 0)[1] = (byte)green;


                    //RED
                    a = b = d = e = (dataPtrCopy + nChanCopy * 0 + stepCopy * 0)[2];
                    c = f = (dataPtrCopy + nChanCopy * 1 + stepCopy * 0)[2];
                    g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * 1)[2];
                    i = (dataPtrCopy + nChanCopy * 1 + stepCopy * 1)[2];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    red = sX + sY;

                    red = (red > 255) ? 255 : red;
                    red = (red < 0) ? 0 : red;

                    (dataPtr + nChan * 0 + step * 0)[2] = (byte)red;



                    //UP RIGHT

                    //BLUE
                    b = c = e = f = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[0];
                    h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[0];
                    a = d = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[0];
                    g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[0];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    blue = sX + sY;

                    blue = (blue > 255) ? 255 : blue;
                    blue = (blue < 0) ? 0 : blue;

                    (dataPtr + nChan * (width - 1) + step * 0)[0] = (byte)blue;


                    //GREEN
                    b = c = e = f = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[1];
                    h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[1];
                    a = d = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[1];
                    g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[1];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    green = sX + sY;

                    green = (green > 255) ? 255 : green;
                    green = (green < 0) ? 0 : green;

                    (dataPtr + nChan * (width - 1) + step * 0)[1] = (byte)green;


                    //RED
                    b = c = e = f = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 0)[2];
                    h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * 1)[2];
                    a = d = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 0)[2];
                    g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * 1)[2];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    red = sX + sY;

                    red = (red > 255) ? 255 : red;
                    red = (red < 0) ? 0 : red;

                    (dataPtr + nChan * (width - 1) + step * 0)[2] = (byte)red;



                    //BOTTOM LEFT

                    //BLUE
                    d = e = g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[0];
                    a = b = (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[0];
                    f = i = (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[0];
                    c = (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[0];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    blue = sX + sY;

                    blue = (blue > 255) ? 255 : blue;
                    blue = (blue < 0) ? 0 : blue;

                    (dataPtr + nChan * 0 + step * (height - 1))[0] = (byte)blue;


                    //GREEN
                    d = e = g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[1];
                    a = b = (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[1];
                    f = i = (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[1];
                    c = (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[1];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    green = sX + sY;

                    green = (green > 255) ? 255 : green;
                    green = (green < 0) ? 0 : green;

                    (dataPtr + nChan * 0 + step * (height - 1))[1] = (byte)green;


                    //RED
                    d = e = g = h = (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 1))[2];
                    a = b = (dataPtrCopy + nChanCopy * 0 + stepCopy * (height - 2))[2];
                    f = i = (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 1))[2];
                    c = (dataPtrCopy + nChanCopy * 1 + stepCopy * (height - 2))[2];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    red = sX + sY;

                    red = (red > 255) ? 255 : red;
                    red = (red < 0) ? 0 : red;

                    (dataPtr + nChan * 0 + step * (height - 1))[2] = (byte)red;



                    //BOTTOM RIGHT

                    //BLUE
                    e = f = h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[0];
                    b = c = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[0];
                    d = g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[0];
                    a = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[0];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    blue = sX + sY;

                    blue = (blue > 255) ? 255 : blue;
                    blue = (blue < 0) ? 0 : blue;

                    (dataPtr + nChan * (width - 1) + step * (height - 1))[0] = (byte)blue;


                    //GREEN
                    e = f = h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[1];
                    b = c = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[1];
                    d = g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[1];
                    a = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[1];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    green = sX + sY;

                    green = (green > 255) ? 255 : green;
                    green = (green < 0) ? 0 : green;

                    (dataPtr + nChan * (width - 1) + step * (height - 1))[1] = (byte)green;


                    //RED
                    e = f = h = i = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 1))[2];
                    b = c = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (height - 2))[2];
                    d = g = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 1))[2];
                    a = (dataPtrCopy + nChanCopy * (width - 2) + stepCopy * (height - 2))[2];

                    sX = Math.Abs((a + 2 * d + g) - (c + 2 * f + i));
                    sY = Math.Abs((g + 2 * h + i) - (a + 2 * b + c));

                    red = sX + sY;

                    red = (red > 255) ? 255 : red;
                    red = (red < 0) ? 0 : red;

                    (dataPtr + nChan * (width - 1) + step * (height - 1))[2] = (byte)red;
                }
            }
        }
        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int nChanCopy = mCopy.nChannels;
                int x, y;
                int pixel, right, bottom;
                int blue, green, red;
                int step = m.widthStep;
                int stepCopy = mCopy.widthStep;


                if (nChan == 3) // image in RGB
                {

                    for (y = 0; y < height - 1; y++)
                    {
                        for (x = 0; x < width - 1; x++)
                        {
                            //BLUE
                            pixel = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            right = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            bottom = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];

                            blue = Math.Abs(pixel - right) + Math.Abs(pixel - bottom);

                            blue = (blue < 0) ? 0 : blue;
                            blue = (blue > 255) ? 255 : blue;

                            (dataPtr + nChan * x + step * y)[0] = (byte)blue;


                            //GREEN
                            pixel = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            right = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            bottom = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];

                            green = Math.Abs(pixel - right) + Math.Abs(pixel - bottom);

                            green = (green < 0) ? 0 : green;
                            green = (green > 255) ? 255 : green;

                            (dataPtr + nChan * x + step * y)[1] = (byte)green;


                            //RED
                            pixel = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            right = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            bottom = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];

                            red = Math.Abs(pixel - right) + Math.Abs(pixel - bottom);

                            red = (red < 0) ? 0 : red;
                            red = (red > 255) ? 255 : red;

                            (dataPtr + nChan * x + step * y)[2] = (byte)red;

                        }
                    }

                    // HORIZONTAL

                    //DOWN
                    for (x = 0; x < width - 1; x++)
                    {
                        //BLUE
                        pixel = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[0];
                        right = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[0];

                        blue = Math.Abs(pixel - right);

                        blue = (blue < 0) ? 0 : blue;
                        blue = (blue > 255) ? 255 : blue;

                        (dataPtr + nChan * x + step * (height - 1))[0] = (byte)blue;


                        //GREEN
                        pixel = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[1];
                        right = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[1];

                        green = Math.Abs(pixel - right);

                        green = (green < 0) ? 0 : green;
                        green = (green > 255) ? 255 : green;

                        (dataPtr + nChan * x + step * (height - 1))[1] = (byte)green;


                        //RED
                        pixel = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[2];
                        right = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[2];

                        red = Math.Abs(pixel - right);

                        red = (red < 0) ? 0 : red;
                        red = (red > 255) ? 255 : red;

                        (dataPtr + nChan * x + step * (height - 1))[2] = (byte)red;
                    }


                    for (y = 0; y < height - 1; y++)
                    {
                        //BLUE
                        pixel = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[0];
                        bottom = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[0];

                        blue = Math.Abs(pixel - bottom);

                        blue = (blue < 0) ? 0 : blue;
                        blue = (blue > 255) ? 255 : blue;

                        (dataPtr + nChan * (width - 1) + step * y)[0] = (byte)blue;


                        //GREEN
                        pixel = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[1];
                        bottom = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[1];

                        green = Math.Abs(pixel - bottom);

                        green = (green < 0) ? 0 : green;
                        green = (green > 255) ? 255 : green;

                        (dataPtr + nChan * (width - 1) + step * y)[1] = (byte)green;


                        //RED
                        pixel = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[2];
                        bottom = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[2];

                        red = Math.Abs(pixel - bottom);

                        red = (red < 0) ? 0 : red;
                        red = (red > 255) ? 255 : red;

                        (dataPtr + nChan * (width - 1) + step * y)[2] = (byte)red;
                    }



                    (dataPtr + nChan * (width - 1) + step * (height - 1))[0] = (byte)0;
                    (dataPtr + nChan * (width - 1) + step * (height - 1))[1] = (byte)0;
                    (dataPtr + nChan * (width - 1) + step * (height - 1))[2] = (byte)0;
                }
            }
        }

        // Aula 7
        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {

                // obter apontador do inicio da imagem
                MIplImage m = img.MIplImage; // imagem de destino
                MIplImage mCopy = imgCopy.MIplImage; // imagem original

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the destiny image

                int y, x, i;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int step = m.widthStep;
                int nChanCopy = mCopy.nChannels;
                int stepCopy = mCopy.widthStep;

                // Core
                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < width - 1; x++)
                    {
                        double[] distanciasCore =
                        {
                           
                            // 0
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 1
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 2 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                
                            // 3 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                 
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 6 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                
                            // 7 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 8 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2))
                        };

                        int minCore = Array.IndexOf(distanciasCore, distanciasCore.Min());

                        switch (minCore)
                        {
                            case 0:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2];
                                break;

                            case 1:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                                break;

                            case 2:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2];
                                break;

                            case 3:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                                break;

                            case 4:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                                break;

                            case 5:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                                break;

                            case 6:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2];
                                break;

                            case 7:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                                break;

                            case 8:
                                (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0];
                                (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1];
                                (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2];
                                break;

                            default:
                                break;
                        }
                    }
                }

                // HORIZONTAL
                for (x = 0; x < width - 1; x++)
                {

                    // UP
                    double[] distanciasH =
                       {
                           
                            // 0
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 1
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x ) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 2 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                               // AQUIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII 
                            // 3 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x ) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                 
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 6 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                
                            // 7 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 8 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2))
                        };



                    int min = Array.IndexOf(distanciasH, distanciasH.Min());

                    switch (min)
                    {
                        case 0:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 1:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 2:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            break;

                        case 3:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 4:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 5:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            break;

                        case 6:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2];
                            break;

                        case 7:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                            break;

                        case 8:
                            (dataPtr + nChan * x + step * 0)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * x + step * 0)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * x + step * 0)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2];
                            break;

                        default:
                            break;

                    }

                    //BOTTOM

                    double[] distanciasB =
                        {
                           
                            // 0
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 1
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 2 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                
                            // 3 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                 
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 6 
                            0,
                                
                            // 7 
                            0,
                           
                            // 8 
                            0
                        };

                    distanciasB[6] = distanciasB[3];
                    distanciasB[7] = distanciasB[4];
                    distanciasB[8] = distanciasB[5];


                    min = Array.IndexOf(distanciasB, distanciasB.Min());

                    switch (min)
                    {
                        case 0:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 1:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 2:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            break;

                        case 3:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 4:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 5:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            break;

                        case 6:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 7:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 8:
                            (dataPtr + nChan * x + step * (height - 1))[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * (height - 1))[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * (height - 1))[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            break;

                        default:
                            break;

                    }

                }

                //VERTICAL 
                for (y = 0; y < height - 1; y++)
                {
                    //LEFT

                    double[] distanciasL =
                       {
                           
                            // 0
                            0,

                            // 1
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 2 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                
                            // 3 
                            0,
                            
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 6 
                            0,
                                
                            // 7 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 8 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2))
                        };

                    distanciasL[0] = distanciasL[1];
                    distanciasL[3] = distanciasL[4];
                    distanciasL[6] = distanciasL[7];

                    int min = Array.IndexOf(distanciasL, distanciasL.Min());

                    switch (min)
                    {
                        case 0:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            break;

                        case 1:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            break;

                        case 2:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2];
                            break;

                        case 3:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 4:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 5:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            break;

                        case 6:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                            break;

                        case 7:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                            break;

                        case 8:
                            (dataPtr + nChan * 0 + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * 0 + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * 0 + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2];
                            break;

                        default:
                            break;
                    }

                    //RIGHT
                    double[] distanciasR =
                        {
                           
                            // 0
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 1
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 2 
                            0,
                                
                            // 3 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                 
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            0,
                           
                            // 6 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                
                            // 7 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 8 
                            0
                        };

                    distanciasR[2] = distanciasR[1];
                    distanciasR[5] = distanciasR[4];
                    distanciasR[8] = distanciasR[7];

                    min = Array.IndexOf(distanciasR, distanciasR.Min());

                    switch (min)
                    {
                        case 0:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2];
                            break;

                        case 1:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            break;

                        case 2:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            break;

                        case 3:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 4:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 5:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 6:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2];
                            break;

                        case 7:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                            break;

                        case 8:
                            (dataPtr + nChan * (width - 1) + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * (width - 1) + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * (width - 1) + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                            break;

                        default:
                            break;
                    }


                }


                // TOP LEFT
                {
                    double[] distanciasTL =
                            {
                           
                            // 0
                            0,

                            // 1
                            0,

                            // 2 
                            0,
                                
                            // 3 
                            0,
                                 
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 6 
                            0,
                                
                            // 7 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 8 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2))
                        };
                    distanciasTL[0] = distanciasTL[4];
                    distanciasTL[1] = distanciasTL[4];
                    distanciasTL[2] = distanciasTL[5];
                    distanciasTL[3] = distanciasTL[4];
                    distanciasTL[6] = distanciasTL[7];
                }


                // TOP RIGHT
                {
                    double[] distanciasTR =
                            {
                           
                            // 0
                            0,

                            // 1
                            0,

                            // 2 
                            0,
                                
                            // 3 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                 
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            0,
                           
                            // 6 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                
                            // 7 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 8 
                            0
                        };
                    distanciasTR[0] = distanciasTR[3];
                    distanciasTR[1] = distanciasTR[4];
                    distanciasTR[2] = distanciasTR[4];
                    distanciasTR[5] = distanciasTR[4];
                    distanciasTR[8] = distanciasTR[7];

                    int min1 = Array.IndexOf(distanciasTR, distanciasTR.Min());

                    switch (min1)
                    {
                        case 0:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 1:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 2:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 3:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 4:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 5:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 6:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2];
                            break;

                        case 7:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                            break;

                        case 8:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];
                            break;

                        default:
                            break;
                    }
                }


                // BOTTOM LEFT
                {
                    double[] distanciasBL =
                                {
                           
                            // 0
                            0,

                            // 1
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 2 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                
                            // 3
                            0,
                                 
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                           
                            // 6 
                            0,

                            //7
                            0,

                            //8
                            0
                        };

                    distanciasBL[0] = distanciasBL[1];
                    distanciasBL[3] = distanciasBL[4];
                    distanciasBL[6] = distanciasBL[4];
                    distanciasBL[7] = distanciasBL[4];
                    distanciasBL[8] = distanciasBL[5];



                    int minBL = Array.IndexOf(distanciasBL, distanciasBL.Min());

                    switch (minBL)
                    {
                        case 0:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            break;

                        case 1:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            break;

                        case 2:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2];
                            break;

                        case 3:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 4:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 5:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            break;

                        case 6:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 7:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 8:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            break;

                        default:
                            break;
                    }
                }


                // BOTTOM RIGHT
                {
                    double[] distanciasBR =
                            {
                           
                            // 0
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2))+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 1
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),

                            // 2 
                            0,
                                
                            // 3 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                                 
                            // 4 
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y - 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y + 1))[2], 2) )+
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x) + stepCopy * (y + 1))[2], 2)) +
                            Math.Sqrt(Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[0] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[1] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1], 2) + Math.Pow((dataPtrCopy + nChanCopy * (x) + stepCopy * (y))[2] - (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2], 2)),
                              
                            // 5 
                            0,
                           
                            // 6 
                            0,

                            //7
                            0,

                            //8
                            0
                        };


                    distanciasBR[2] = distanciasBR[1];
                    distanciasBR[5] = distanciasBR[4];
                    distanciasBR[6] = distanciasBR[3];
                    distanciasBR[7] = distanciasBR[4];
                    distanciasBR[8] = distanciasBR[4];




                    int minBR = Array.IndexOf(distanciasBR, distanciasBR.Min());

                    switch (minBR)
                    {
                        case 0:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * (y - 1))[2];
                            break;

                        case 1:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            break;

                        case 2:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * (y - 1))[2];
                            break;

                        case 3:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 4:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 5:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 6:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * (x - 1) + stepCopy * y)[2];
                            break;

                        case 7:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        case 8:
                            (dataPtr + nChan * x + step * y)[0] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            (dataPtr + nChan * x + step * y)[1] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            (dataPtr + nChan * x + step * y)[2] = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            break;

                        default:
                            break;
                    }
                }
            }
        }


        // Histogram Gray

        public static int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion topY left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte blue, green, red, gray;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;


                int step = m.widthStep;

                int[] array = new int[256];
                int pos;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            pos = (int)Math.Round((
                                (dataPtr + nChan * x + step * y)[0] +
                                (dataPtr + nChan * x + step * y)[1] +
                                (dataPtr + nChan * x + step * y)[2]) / 3.0);

                            array[pos]++;
                        }
                    }
                }
                return array;
            }
        }

        // Histogram RGB Facultativo

        public static int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte blue, green, red;

                int[,] values = new int[3, 256];
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {

                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            values[0, blue]++;
                            values[1, green]++;
                            values[2, red]++;


                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
                return values;
            }
        }

        // Histogram All Facultativo

        public static int[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte blue, green, red;

                int[,] values = new int[4, 256];
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int valorPixel;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {

                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];
                            valorPixel = (int)Math.Round((blue + green + red) / 3.0);



                            values[1, blue]++;
                            values[2, green]++;
                            values[3, red]++;
                            values[0, valorPixel]++;


                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
                return values;
            }
        }

        // Roberts Facultativo
        public static void Roberts(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int nChanCopy = mCopy.nChannels;
                int x, y;
                int pixel, diagnlUpRight, diagnlBottomRight, diagnlBottomLeft;
                int blue, green, red;
                int step = m.widthStep;
                int stepCopy = mCopy.widthStep;


                if (nChan == 3) // image in RGB
                {

                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //BLUE
                            pixel = (dataPtrCopy + nChanCopy * x + stepCopy * y)[0];
                            diagnlUpRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                            diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0];
                            diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];

                            blue = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                            blue = (blue < 0) ? 0 : blue;
                            blue = (blue > 255) ? 255 : blue;

                            (dataPtr + nChan * x + step * y)[0] = (byte)blue;


                            //GREEN
                            pixel = (dataPtrCopy + nChanCopy * x + stepCopy * y)[1];
                            diagnlUpRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                            diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1];
                            diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];

                            green = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                            green = (green < 0) ? 0 : green;
                            green = (green > 255) ? 255 : green;

                            (dataPtr + nChan * x + step * y)[1] = (byte)green;


                            //RED
                            pixel = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                            diagnlUpRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                            diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2];
                            diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];

                            red = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                            red = (red < 0) ? 0 : red;
                            red = (red > 255) ? 255 : red;

                            (dataPtr + nChan * x + step * y)[2] = (byte)red;

                        }

                    }

                    //HORIZONTAL

                    //DOWN


                    for (x = 0; x < width - 1; x++)
                    {
                        //BLUE
                        pixel = diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[0];
                        diagnlUpRight = diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[0];
                        //diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0];
                        //  diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[0];

                        blue = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                        blue = (blue < 0) ? 0 : blue;
                        blue = (blue > 255) ? 255 : blue;

                        (dataPtr + nChan * x + step * (height - 1))[0] = (byte)blue;


                        //GREEN
                        pixel = diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[1];
                        diagnlUpRight = diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[1];
                        /*    pixel = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[1];
                            diagnlUpRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[1];
                            diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1];
                            diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];*/

                        green = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                        green = (green < 0) ? 0 : green;
                        green = (green > 255) ? 255 : green;

                        (dataPtr + nChan * x + step * (height - 1))[1] = (byte)green;


                        //RED
                        pixel = diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[2];
                        diagnlUpRight = diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[2];
                        /*      pixel = (dataPtrCopy + nChanCopy * x + stepCopy * (height - 1))[2];
                              diagnlUpRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (height - 1))[2];
                              diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2];
                              diagnlBottomLeft = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];*/

                        red = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                        red = (red < 0) ? 0 : red;
                        red = (red > 255) ? 255 : red;

                        (dataPtr + nChan * x + step * (height - 1))[2] = (byte)red;
                    }





                    for (y = 0; y < height - 1; y++)
                    {
                        pixel = diagnlUpRight = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[0];
                        //  diagnlUpRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[0];
                        // diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[0];
                        diagnlBottomLeft = diagnlBottomRight = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * (y + 1))[0];

                        blue = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                        blue = (blue < 0) ? 0 : blue;
                        blue = (blue > 255) ? 255 : blue;

                        (dataPtr + nChan * (width - 1) + step * y)[0] = (byte)blue;


                        //GREEN
                        pixel = diagnlUpRight = (dataPtrCopy + nChanCopy * (width - 1) + stepCopy * y)[1];
                        // diagnlUpRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[1];
                        // diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[1];
                        diagnlBottomLeft = diagnlBottomRight = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[1];

                        green = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                        green = (green < 0) ? 0 : green;
                        green = (green > 255) ? 255 : green;

                        (dataPtr + nChan * (width - 1) + step * y)[1] = (byte)green;


                        //RED
                        pixel = diagnlUpRight = (dataPtrCopy + nChanCopy * x + stepCopy * y)[2];
                        //   diagnlUpRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * y)[2];
                        //  diagnlBottomRight = (dataPtrCopy + nChanCopy * (x + 1) + stepCopy * (y + 1))[2];
                        diagnlBottomLeft = diagnlBottomRight = (dataPtrCopy + nChanCopy * x + stepCopy * (y + 1))[2];

                        red = Math.Abs(pixel - diagnlBottomRight) + Math.Abs(diagnlUpRight - diagnlBottomLeft);

                        red = (red < 0) ? 0 : red;
                        red = (red > 255) ? 255 : red;

                        (dataPtr + nChan * (width - 1) + step * y)[2] = (byte)red;
                    }

                    (dataPtr + nChan * (width - 1) + step * (height - 1))[0] = (byte)0;
                    (dataPtr + nChan * (width - 1) + step * (height - 1))[1] = (byte)0;
                    (dataPtr + nChan * (width - 1) + step * (height - 1))[2] = (byte)0;


                }
            }
        }



        // ConvertToBW
        public static void ConvertToBW(Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {
                int[] histograma = new int[256];
                int width = img.Width;
                int height = img.Height;
                MIplImage mUndo = img.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer(); // Pointer to the original image
                byte* dataPtrWrite = (byte*)mUndo.imageData.ToPointer(); // Pointer to the destiny image

                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int sum = (int)Math.Round(((dataPtrRead + nChan * x + widthStep * y)[0]
                            + (dataPtrRead + nChan * x + widthStep * y)[1]
                            + (dataPtrRead + nChan * x + widthStep * y)[2]) / 3.0);  // blue + green + red

                        if (sum > threshold)
                        {
                            (dataPtrWrite + nChan * x + widthStep * y)[0] = 255;
                            (dataPtrWrite + nChan * x + widthStep * y)[1] = 255;
                            (dataPtrWrite + nChan * x + widthStep * y)[2] = 255;
                        }
                        else
                        {
                            (dataPtrWrite + nChan * x + widthStep * y)[0] = 0;
                            (dataPtrWrite + nChan * x + widthStep * y)[1] = 0;
                            (dataPtrWrite + nChan * x + widthStep * y)[2] = 0;
                        }
                    }
                }
            }
        }


        // ConvertToBW_Otsu
        public static void ConvertToBW_Otsu(Image<Bgr, byte> img)
        {
            unsafe
            {
                int t, i;
                double q1, q2, u1, u2;
                int width = img.Width;
                int height = img.Height;
                MIplImage mUndo = img.MIplImage;
                byte* dataPtrWrite = (byte*)mUndo.imageData.ToPointer();
                double[] variancias = new double[256];
                int[] histograma = Histogram_Gray(img);

                for (t = 0; t < 256; t++)
                {
                    q1 = 0;
                    q2 = 0;
                    u1 = 0;
                    u2 = 0;
                    for (i = 0; i <= t; i++)
                    {
                        q1 += histograma[i] / (double)(width * height);
                        u1 += i * histograma[i] / (double)(width * height);
                    }

                    u1 /= q1;

                    for (i = t + 1; i < 256; i++)
                    {
                        q2 += histograma[i] / (double)(width * height);
                        u2 += i * histograma[i] / (double)(width * height);
                    }

                    u2 /= q2;

                    variancias[t] = q1 * q2 * Math.Pow(u1 - u2, 2);
                }
                int max_pos = Array.IndexOf(variancias, variancias.Max());

                ConvertToBW(img, max_pos);
            }
        }
        // Facultativo

        public static void Rotation_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                //obter apontador do inicio da imagem
                MIplImage m = img.MIplImage;//imagem de destino
                MIplImage mUndo = imgCopy.MIplImage;//imagem original

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();//Pointer to the original image
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();//Pointer to the destiny image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = (int)Math.Round((x - (width / 2.0)) * Math.Cos(angle) - ((height / 2.0) - y) * Math.Sin(angle) + (width / 2.0));
                        y0 = (int)Math.Round((height / 2.0) - (x - (width / 2.0)) * Math.Sin(angle) - ((height / 2.0) - y) * Math.Cos(angle));

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
                        {
                            blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                            green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                            red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];

                        }
                        else
                        {
                            blue = green = red = 0;
                        }

                        (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                        (dataPtrWrite + nChan * x + widthStep * y)[2] = red;

                    }

                }

            }
        }
        
        public static void Rotation_BilinearParaBranco(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                //obter apontador do inicio da imagem
                MIplImage m = img.MIplImage;//imagem de destino
                MIplImage mUndo = imgCopy.MIplImage;//imagem original

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();//Pointer to the original image
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();//Pointer to the destiny image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = (int)Math.Round((x - (width / 2.0)) * Math.Cos(angle) - ((height / 2.0) - y) * Math.Sin(angle) + (width / 2.0));
                        y0 = (int)Math.Round((height / 2.0) - (x - (width / 2.0)) * Math.Sin(angle) - ((height / 2.0) - y) * Math.Cos(angle));

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
                        {
                            blue = (dataPtrRead + nChan * x0 + widthStep * y0)[0];
                            green = (dataPtrRead + nChan * x0 + widthStep * y0)[1];
                            red = (dataPtrRead + nChan * x0 + widthStep * y0)[2];

                        }
                        else
                        {
                            blue = green = red = 255;
                        }

                        (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                        (dataPtrWrite + nChan * x + widthStep * y)[2] = red;

                    }

                }

            }
        }

        // Scale_Bilinear Facultrativo
        public static void Scale_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                //obter apontador do inicio da imagem
                MIplImage m = img.MIplImage;//imagem de destino
                MIplImage mUndo = imgCopy.MIplImage;//imagem original

                byte* dataPtrR = (byte*)mUndo.imageData.ToPointer();//Pointer to the original image
                byte* dataPtrW = (byte*)m.imageData.ToPointer();//Pointer to the destiny image

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                byte red, green, blue;
                int x0, y0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = (int)Math.Round(x / scaleFactor);
                        y0 = (int)Math.Round(y / scaleFactor);

                        if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
                        {
                            blue = (dataPtrR + nChan * x0 + widthStep * y0)[0];
                            green = (dataPtrR + nChan * x0 + widthStep * y0)[1];
                            red = (dataPtrR + nChan * x0 + widthStep * y0)[2];

                        }
                        else
                        {
                            blue = green = red = 0;
                        }

                        (dataPtrW + nChan * x + widthStep * y)[0] = blue;
                        (dataPtrW + nChan * x + widthStep * y)[1] = green;
                        (dataPtrW + nChan * x + widthStep * y)[2] = red;

                    }

                }

            }
        }

        // Facultativo
        public static void Scale_point_xy_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrRead = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrWrite = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;
                byte red, green, blue;
                int xO, yO;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        xO = (int)Math.Round(centerX + (x - width / 2) / scaleFactor);
                        yO = (int)Math.Round(centerY + (y - height / 2) / scaleFactor);



                        if (xO >= 0 && xO < width && yO >= 0 && yO < height)
                        {
                            blue = (dataPtrRead + nChan * xO + widthStep * yO)[0];
                            green = (dataPtrRead + nChan * xO + widthStep * yO)[1];
                            red = (dataPtrRead + nChan * xO + widthStep * yO)[2];
                        }
                        else
                        {
                            blue = red = green = 0;
                        }

                        (dataPtrWrite + nChan * x + widthStep * y)[0] = blue;
                        (dataPtrWrite + nChan * x + widthStep * y)[1] = green;
                        (dataPtrWrite + nChan * x + widthStep * y)[2] = red;
                    }
                }



            }
        }
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));
            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }
        public static void Binarization(Image<Bgr, byte> img)
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
                            if (150 < hue && hue < 240)
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
                    int thresholdPertenceTabuleiro = 3;
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

                }
            }
        }



        //public static void BinarizeImageWithColorToHsv(Image<Bgr, byte> img)
        //{
        //    unsafe
        //    {
        //        MIplImage m = img.MIplImage;
        //        byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
        //        byte blue, green, red;

        //        int width = img.Width;
        //        int height = img.Height;
        //        int nChan = m.nChannels; // number of channels - 3
        //        int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
        //        int x, y;
        //        int step = m.widthStep;
        //        float[] histogramX = new float[width]; //holds the percentage of 255 on that X column
        //        float[] histogramY = new float[height]; //holds the percentage of 255 of that Y line
        //        if (nChan == 3) // image in RGB RedGreenBlue
        //        {
        //            for (y = 0; y < height; y++)
        //            {
        //                for (x = 0; x < width; x++)
        //                {
        //                    //retrive 3 colour components
        //                    blue = (dataPtr + x * nChan + y * step)[0];
        //                    green = (dataPtr + x * nChan + y * step)[1];
        //                    red = (dataPtr + x * nChan + y * step)[2];

        //                    Color original = Color.FromArgb(red, green, blue);
        //                    ColorToHSV(original, out var hue, out var saturation, out var value);
        //                    if (70 < hue && hue < 320) //DAR O TUNE AQUI
        //                    {
        //                        histogramY[y] = histogramY[y] + 1;
        //                        histogramX[x] = histogramX[x] + 1;
        //                        (dataPtr + x * nChan + y * step)[0] = 0;
        //                        (dataPtr + x * nChan + y * step)[1] = 0;
        //                        (dataPtr + x * nChan + y * step)[2] = 0;
        //                    }
        //                    else
        //                    {
        //                        (dataPtr + x * nChan + y * step)[0] = 255;
        //                        (dataPtr + x * nChan + y * step)[1] = 255;
        //                        (dataPtr + x * nChan + y * step)[2] = 255;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        public static void BinarizeImageWithColorToHsvBlack(Image<Bgr, byte> img)
        {
            var bcroper = new CropperService();

            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int step = m.widthStep;
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
                            if (value < 0.25) //DAR O TUNE AQUI
                            {

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
                }
            }
        }

        public static (double, string) CalculateIgualityDiference(Image<Bgr, byte> imagem, Image<Bgr, byte> imagemBD, string nomeImgBd)
        {
            unsafe
            {
                //Imagem que vai ser comparada
                MIplImage m = imagem.MIplImage;
                byte* dataPtrImagem = (byte*)m.imageData.ToPointer(); // Pointer to the image


                //Imagem da base de dados
                MIplImage mBD = imagemBD.MIplImage;
                byte* dataPtrImagemBD = (byte*)mBD.imageData.ToPointer(); // Pointer to the image

                byte blue, green, red;

                double percentagemIgualidade = 0;
                int numeroPixeisIguais = 0;
                int width = imagem.Width;
                int height = imagem.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                int step = m.widthStep;
                int pixelOriginal = 0;
                int pixelBD = 0;
                int diferenca = 0;
                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = (byte)(int)Math.Round((double)(dataPtrImagem + nChan * x + step * y)[0]);
                            green = (byte)(int)Math.Round((double)(dataPtrImagem + nChan * x + step * y)[1]);
                            red = (byte)(int)Math.Round((double)(dataPtrImagem + nChan * x + step * y)[2]);

                            //retrive 3 colour components Base de Dados
                            var blueBD = (byte)(int)Math.Round((double)(dataPtrImagemBD + nChan * x + step * y)[0]);
                            var greenBD = (byte)(int)Math.Round((double)(dataPtrImagemBD + nChan * x + step * y)[1]);
                            var redBD = (byte)(int)Math.Round((double)(dataPtrImagemBD + nChan * x + step * y)[2]);

                            pixelOriginal = (blue == 0 && green == 0 && red == 0) ? 1 : 0;
                            pixelBD = (blueBD == 0 && greenBD == 0 && redBD == 0) ? 1 : 0;
                            diferenca += Math.Abs(pixelOriginal - pixelBD);
                        }
                    }
                }
                return (diferenca, nomeImgBd);
            }
        }

        public static string GetNomePeca(Image<Bgr, byte> figure)
        {
            string[] Base_Dados = Directory.GetFiles(Helper.FolderPath("BD Chess"));

            int aux = Base_Dados.Length;
            Image<Bgr, byte> img_BD;
            (double, string)[] relacoes = new (double, string)[aux];

            

            //PERCORRER BASE DE DADOS
            for (int B_D = 0; B_D < aux; B_D++)
            {
                img_BD = new Image<Bgr, Byte>(Base_Dados[B_D]);
                var figureBd = new CropperService().CropFigure(img_BD);

                var figureResized = figure.Resize(128, 128, INTER.CV_INTER_CUBIC);
                var imgDbResized = figureBd.Resize(128, 128, INTER.CV_INTER_CUBIC);

                //Passo para debug
                Helper.SaveImagesLocally(figureResized, imgDbResized);
               
                relacoes[B_D] = CalculateIgualityDiference(figureResized, imgDbResized, Path.GetFileNameWithoutExtension(Base_Dados[B_D])); //percentagens de igualdade
            }
            string path = Base_Dados[Array.IndexOf(relacoes, relacoes.Min())];
            return Path.GetFileNameWithoutExtension(path); //o Nome da peca correspondente
        }

    }
}
