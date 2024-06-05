using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CG_OpenCV.Models;
using CG_OpenCV.Services;
using Emgu.CV;
using Emgu.CV.Structure;
using static System.Net.Mime.MediaTypeNames;

namespace CG_OpenCV
{
    public partial class MainForm : Form
    {
        Image<Bgr, Byte> img = null; // working image
        Image<Bgr, Byte> imgRefresher = null; // working image
        Image<Bgr, Byte> croppedBoard = null; // working image
        Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
        string title_bak = "";
        

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            PopulateComboBox();
        }

        private void PopulateComboBox()
        {
            // Column labels for a chessboard
            char[] columns = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

            // Loop through each row and column to generate board squares
            foreach (char column in columns)
            {
                for (int row = 1; row <= 8; row++)
                {
                    string square = $"{column}{row}";
                    comboBox1.Items.Add(square);
                }
            }
        }

    /// <summary>
    /// Opens a new image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                Text = title_bak + " [" +
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
                        "]";
                imgUndo = img.Copy();
                ImageViewer.Image = img.Bitmap;
                ImageViewer.Refresh();
            }
        }

        /// <summary>
        /// Saves an image with a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageViewer.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// restore last undo copy of the working image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUndo == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Change visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
                ImageViewer.Dock = DockStyle.Fill;
            }
            else // with scroll bars
            {
                ImageViewer.Dock = DockStyle.None;
                ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Show authors form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Calculate the image negative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Negative(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Call automated image processing check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvalForm eval = new EvalForm();
            eval.ShowDialog();
        }

        /// <summary>
        /// Call image convertion to gray scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToGray(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }
        private void binarizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToBW_Otsu(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void cropTabuleiroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var cropperService = new CropperService();
            //var croppedBoard = cropperService.CropBoardAndSaveImage(img);
            //var imagensCortadinhas = cropperService.CropHouseFromBoard(croppedBoard);
        }

        private void hSVPretoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string[] pecasBd = Directory.GetFiles(Helper.FolderPath("BD Chess"));
            //Helper.BinarizeAndSaveBDImages(pecasBd);

            //string[] pecasCortadinhas = Directory.GetFiles(Helper.FolderPath("ImagensCortadinhas"));
            //Helper.BinarizeAndSaveImages(pecasCortadinhas);
        }

        private void qualPecaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 


            //MessageBox.Show(ImageClass.GetNomePeca(img),
            //               "Success",
            //               MessageBoxButtons.OK,
            //               MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            var casa = comboBox1.SelectedItem?.ToString() ?? null;
            if (string.IsNullOrEmpty(casa) || img == null)
            {
                MessageBox.Show("Por favor selecione uma casa do tabuleiro.",
                          "Warning",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning);
                return;
            }

            var boardCropper = new CropperService();
            croppedBoard = boardCropper.CropBoard(img, out var cSuperior, out var cInferior);
            //ImageViewer.Image = croppedBoard.Bitmap;
            //ImageViewer.Image = img.Bitmap;

            //Atualizar coordenadas em pixeis do tabuleiro
            this.coordSuperior.Text = cSuperior.ToString();
            this.coordInferior.Text = cInferior.ToString();

            string relativePath = Path.Combine("..", "..", "dizerTipoPeca/croppedBoard.png");
            string absolutePath = Path.GetFullPath(relativePath);
            croppedBoard.Bitmap.Save(absolutePath, ImageFormat.Png);

            var croppedHouse = boardCropper.CropBoardHouse(croppedBoard, casa);

            relativePath = Path.Combine("..", "..", "dizerTipoPeca/croppedHouse.png");
            absolutePath = Path.GetFullPath(relativePath);
            croppedHouse.Bitmap.Save(absolutePath, ImageFormat.Png);


            if (croppedHouse == null)
            {
                MessageBox.Show("Casa não encontrada.",
                          "Warning",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning);
                return;
            }

            var pieceFigure = boardCropper.CropFigure(croppedHouse);
            relativePath = Path.Combine("..", "..", $"dizerTipoPeca/croppedFigure.png");
            absolutePath = Path.GetFullPath(relativePath);
            pieceFigure.Bitmap.Save(absolutePath, ImageFormat.Png);

            //todo: verificar se e uma casa vazia

            MessageBox.Show(ImageClass.GetNomePeca(pieceFigure),
                           "Success",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);

        }
    }
}
