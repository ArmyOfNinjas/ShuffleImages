using OnnxObjectDetection;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImageObjDetection.API.Models
{
	public class ImageMetaOutput
	{
		public int Id { get; set; }
		public List<BoundingBox> FilteredBoxes { get; set; }
		public RectangleF UnionBox { get; set; }
		public string FileName { get; set; }
		public Size ImgSize { get; set; }

		public int Top { get; set; }
		public int Right { get; set; }
		public int Bottom { get; set; }
		public int Left { get; set; }
		public List<ImageMetaOutput> Neightbours { get; set; } = new List<ImageMetaOutput>();

	}
}
