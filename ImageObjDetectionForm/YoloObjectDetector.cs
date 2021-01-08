using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alturos.Yolo;
using Alturos.Yolo.Model;


namespace ImageObjDetectionForm
{
	public class YoloObjectDetector
	{
		public MemoryStream DetectObjects(MemoryStream stream)
		{
			MemoryStream updatedStream =  DetectObject(stream);	
			return updatedStream;
		}

		public MemoryStream DetectObject(MemoryStream stream)
		{
			MemoryStream updatedStream = null;
			var configurationDetector = new YoloConfigurationDetector();
			var config = configurationDetector.Detect();
			using (var yoloWrapper = new YoloWrapper(config))
			{
				var items = yoloWrapper.Detect(stream.ToArray()).ToList();
				using (stream)
				{
					updatedStream = AddDetailsToPictureBox(stream, items);
				}
				//items[0].Type -> "Person, Car, ..."
				//items[0].Confidence -> 0.0 (low) -> 1.0 (high)
				//items[0].X -> bounding box
				//items[0].Y -> bounding box
				//items[0].Width -> bounding box
				//items[0].Height -> bounding box
			}


			//var configurationDetector = new YoloConfigurationDetector();
			//var config = configurationDetector.Detect();
			//var yolo = new YoloWrapper(config);
			//using (stream)
			//{
			//	var items = yolo.Detect(stream.ToArray()).ToList();
			//	updatedStream = AddDetailsToPictureBox(stream, items);
			//}

			return updatedStream;
		}

		private MemoryStream AddDetailsToPictureBox(MemoryStream stream, IEnumerable<YoloItem> items)
		{
			Image image = Image.FromStream(stream);
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

			MemoryStream updatedStream = new MemoryStream();
			image.Save(updatedStream, System.Drawing.Imaging.ImageFormat.Jpeg);

			return updatedStream;
		}
	}
}
