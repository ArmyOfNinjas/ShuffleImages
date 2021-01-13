using OnnxObjectDetection;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImageObjDetection.API.Models
{
	public class ImageMetaOutput
	{
		public List<BoundingBox> FilteredBoxes { get; set; }
		public string FileName { get; set; }
		public Size ImgSize { get; set; }
	}
}
