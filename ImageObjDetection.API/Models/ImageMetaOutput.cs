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

		public List<Orientation> BoxOrientations { get; set; } = new List<Orientation>();
		public List<Orientation> BoxCollisions { get; set; } = new List<Orientation>();

		public int Top { get; set; }
		public int Right { get; set; }
		public int Bottom { get; set; }
		public int Left { get; set; }
		public List<ImageMetaOutput> Neightbours { get; set; } = new List<ImageMetaOutput>();
		public Dictionary<Orientation, ImageMetaOutput> Neightbours2 { get; set; } = new Dictionary<Orientation, ImageMetaOutput>();

	}

	public enum Orientation
	{
		Top = 0,
		Right = 1,
		Bottom = 2,
		Left = 3
	}
}
