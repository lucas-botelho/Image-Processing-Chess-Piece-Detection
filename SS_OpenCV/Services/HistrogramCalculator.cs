using CG_OpenCV.Models;
using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CG_OpenCV.Services
{
    internal class HistrogramCalculator
    {
        public List<PieceHistogram> PieceHistograms { get; set; }
        public string[] ImagesPaths { get; set; }
        public HistrogramCalculator(string[] imagesPaths)
        {
            this.PieceHistograms = new List<PieceHistogram>();
            this.ImagesPaths = imagesPaths;
        }

        public List<PieceHistogram> Calculate()
        {
            foreach (var filePath in ImagesPaths)
            {
                try
                {
                    Image<Bgr, byte> img = new Image<Bgr, byte>(filePath);
                    int[,] histogram = ImageClass.Histogram_RGB(img);      
                    var imgName = Path.GetFileName(filePath);
                    PrintHistogram(histogram, imgName);
                    PieceHistograms.Add(new PieceHistogram() { HistogramValueRGB = histogram , Name = imgName });
                }
                catch (Exception ex) { throw ex; }

            }

            return this.PieceHistograms;
        }

        private void PrintHistogram(int[,] histogram, string imgName)
        {
            Console.WriteLine($"IMAGEM:{imgName}");
            for (int i = 0; i < 256; i++)
            {
                Console.WriteLine($"Value {i}: B={histogram[0, i]} G={histogram[1, i]} R={histogram[2, i]}");
            }
        }

    }
}
