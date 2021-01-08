using Alturos.Yolo;
using Alturos.Yolo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageObjDetectionForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files |*.jpg;*png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                picImage.Image = Image.FromFile(ofd.FileName);
            }
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            var configurationDetector = new YoloConfigurationDetector();
            var config = configurationDetector.Detect();
            var yolo = new YoloWrapper(config);
            var memoryStream = new MemoryStream();
            picImage.Image.Save(memoryStream, ImageFormat.Png);

            YoloObjectDetector yoloObjectDetector = new YoloObjectDetector();
            MemoryStream updatedStream = yoloObjectDetector.DetectObject(memoryStream);
            using (updatedStream)
            {
                picImage.Image = Image.FromStream(updatedStream);
            }

            //var items = yolo.Detect(memoryStream.ToArray()).ToList();

            //AddDetailsToPictureBox(picImage, items);
        }

        void AddDetailsToPictureBox(PictureBox pictureBoxToRender, List<YoloItem> items)
        {
            var image = pictureBoxToRender.Image;
            var font = new Font("Arial", 15, FontStyle.Bold);
            var brush = new SolidBrush(Color.Red);
            var graphics = Graphics.FromImage(image);

            foreach (var item in items)
            {
                var x = item.X;
                var y = item.Y;
                var width = item.Width;
                var height = item.Height;

                var rect = new Rectangle(x, y, width, height);
                var pen = new Pen(Color.Red, 2);

                graphics.DrawRectangle(pen, rect);
                graphics.DrawString(item.Type, font, brush, new Point(x, y));
            }
            pictureBoxToRender.Image = image;
        }
    }
}
