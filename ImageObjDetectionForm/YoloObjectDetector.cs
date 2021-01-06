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

		//public List<MemoryStream> DetectObjects(List<MemoryStream> streams)
		//{
		//	List<MemoryStream> updatedStreams = new List<MemoryStream>();
		//	for (int i = 0; i < streams.Count; i++)
		//	{
		//		updatedStreams.Add(DetectObject(streams[i]));
		//	}
		//	return updatedStreams;
		//}

		private MemoryStream DetectObject(MemoryStream stream)
		{
			MemoryStream updatedStream = null;
			var configurationDetector = new ConfigurationDetector();
			var config = configurationDetector.Detect();
			var yolo = new YoloWrapper(config);
			//var memoryStream = new MemoryStream();
			//picImage.Image.Save(memoryStream, ImageFormat.Png);
			using (stream)
			{
				var items = yolo.Detect(stream.ToArray()).ToList();
				updatedStream = AddDetailsToPictureBox(stream, items);
			}
			return updatedStream;
		}

		private MemoryStream AddDetailsToPictureBox(MemoryStream stream, List<YoloItem> items)
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
