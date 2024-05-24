using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_OpenCV.Models
{
    internal class PieceHistogram
    {
        public string Name { get; set; }
        public int[,] HistogramValueRGB { get; set; }
    }
}
