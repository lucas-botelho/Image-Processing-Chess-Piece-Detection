using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_OpenCV.Services
{
    internal static class Helper
    {
        public static void ProcessAndSaveImages(Image<Bgr, byte> img, Image<Bgr, byte> img_BD)
        {
            try
            {
                string relativeSavingPath1 = Path.Combine("..", "..", $"dizerTipoPeca/original.png");
                string absoluteSavingPath1 = Path.GetFullPath(relativeSavingPath1);

                string relativeSavingPath2 = Path.Combine("..", "..", $"dizerTipoPeca/bd.png");
                string absoluteSavingPath2 = Path.GetFullPath(relativeSavingPath2);
                // Ensure the directory exists
                string directory = Path.GetDirectoryName(absoluteSavingPath1);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //ImageClass.BinarizeImageWithColorToHsvBlack(img); //Binarizacao das imagems v menos 0.2
                                                                  // Copy the image and save it
                using (var imgOriginalCortadaHsv = img.Copy())
                {
                    imgOriginalCortadaHsv.Bitmap.Save(absoluteSavingPath1, ImageFormat.Png);
                }

                //ImageClass.BinarizeImageWithColorToHsvBlack(img_BD);

                // Copy the image and save it
                using (var imgBdHsv = img_BD.Copy())
                {
                    imgBdHsv.Bitmap.Save(absoluteSavingPath2, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static string FolderPath(string folderName)
        {
            string relativePath = Path.Combine("..", "..", folderName);
            return Path.GetFullPath(relativePath);
        }

        public static void BinarizeAndSaveBDImages(string[] pecasBd)
        {
            foreach (var peca in pecasBd)
            {
                try
                {
                    // Load the image
                    Image<Bgr, byte> img = new Image<Bgr, byte>(peca);

                    // Apply the BinarizeImageWithColorToHsc method
                    ImageClass.BinarizeImageWithColorToHsvBlack(img);

                    // Prepare the saving path
                    string relativeSavingPath = Path.Combine("..", "..", $"ImagensBdBinarizadas/{Path.GetFileNameWithoutExtension(peca)}.png");
                    string absoluteSavingPath = Path.GetFullPath(relativeSavingPath);

                    // Ensure the directory exists
                    string directory = Path.GetDirectoryName(absoluteSavingPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Copy the image and save it
                    using (var imgCopy = img.Copy())
                    {
                        imgCopy.Bitmap.Save(absoluteSavingPath, ImageFormat.Png);
                    }

                    Console.WriteLine("Image saved successfully.");
                }
                catch (System.Runtime.InteropServices.ExternalException ex)
                {
                    Console.WriteLine("Error saving image: " + ex.Message);
                }
            }
        }


        public static void BinarizeAndSaveImages(string[] pecasCortadinhas)
        {
            foreach (var peca in pecasCortadinhas)
            {
                try
                {
                    // Load the image
                    Image<Bgr, byte> img = new Image<Bgr, byte>(peca);

                    // Apply the BinarizeImageWithColorToHsvBlack method
                    ImageClass.BinarizeImageWithColorToHsvBlack(img);

                    // Prepare the saving path
                    string relativeSavingPath = Path.Combine("..", "..", $"ImagensCortadinhasBinarizadas/{Path.GetFileNameWithoutExtension(peca)}.png");
                    string absoluteSavingPath = Path.GetFullPath(relativeSavingPath);

                    // Ensure the directory exists
                    string directory = Path.GetDirectoryName(absoluteSavingPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Copy the image and save it
                    using (var imgCopy = img.Copy())
                    {
                        imgCopy.Bitmap.Save(absoluteSavingPath, ImageFormat.Png);
                    }

                    Console.WriteLine("Image saved successfully.");
                }
                catch (System.Runtime.InteropServices.ExternalException ex)
                {
                    Console.WriteLine("Error saving image: " + ex.Message);
                }
            }
        }


    }
}
