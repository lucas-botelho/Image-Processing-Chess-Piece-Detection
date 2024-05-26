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
    internal class PecaImagem
    {
        public Image<Bgr, Byte> Image { get; set; }
        public string Name { get; set; }
        public PieceImageDiscovery PieceImageDiscoveryService { get; set; }

        public PecaImagem()
        {
            this.PieceImageDiscoveryService = new PieceImageDiscovery();
        }

    }
}
