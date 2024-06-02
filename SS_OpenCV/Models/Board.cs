using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CG_OpenCV.Services;

namespace CG_OpenCV.Models
{
    internal class Board
    {
        public PecaImagem[,] ImagesBoardOriginal { get; set; }
        public CropperService BoardCropperService { get; set; }

        public Board(Image<Bgr, Byte> boardInteiro)
        {
        }
    }
}
