using ImageObjDetection.API.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ImageObjDetection.API.v1.Services
{
	public class ImageShuffleService
	{
		private List<ImageMetaOutput> _imgMetaList { get; set; }

		public ImageShuffleService(List<ImageMetaOutput> imgMetaList)
		{
			_imgMetaList = imgMetaList;
		}

		public List<string> Solve()
		{
			UnionFilteredBoxes();
			SetBoxOrientation();






			return new List<string>();
		}

		private List<ImageMetaOutput> UnionFilteredBoxes()
		{
			foreach (var node in _imgMetaList)
			{
				float xMin = node.FilteredBoxes.Min(s => s.Dimensions.X);
				float yMin = node.FilteredBoxes.Min(s => s.Dimensions.Y);
				float xMax = node.FilteredBoxes.Max(s => s.Dimensions.X + s.Dimensions.Width);
				float yMax = node.FilteredBoxes.Max(s => s.Dimensions.Y + s.Dimensions.Height);
				node.UnionBox = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
			}
			return _imgMetaList;
		}



		private List<ImageMetaOutput> SetBoxOrientation()
		{
			foreach (var node in _imgMetaList)
			{
				float cutoff = 0;
				if (node.ImgSize.Width >= node.ImgSize.Height)
				{
					cutoff = (node.ImgSize.Width - node.ImgSize.Height) / 2;
					if (node.UnionBox.Left <= cutoff)
						node.Left = 1;
					if (node.UnionBox.Right >= node.ImgSize.Width - cutoff)
						node.Right = 1;
					if (node.UnionBox.Top <= 0)
						node.Top = 1;
					if (node.UnionBox.Bottom >= node.ImgSize.Height)
						node.Bottom = 1;
				}
				else
				{
					cutoff = (node.ImgSize.Height - node.ImgSize.Width) / 2;
					if (node.UnionBox.Left <= 0)
						node.Left = 1;
					if (node.UnionBox.Right >= node.ImgSize.Width)
						node.Right = 1;
					if (node.UnionBox.Top <= cutoff)
						node.Top = 1;
					if (node.UnionBox.Bottom >= node.ImgSize.Height - cutoff)
						node.Bottom = 1;
				}
			}
			return _imgMetaList;
		}


		private List<ImageMetaOutput> AddNeighbours()
		{
			for (int i = 0; i < _imgMetaList.Count - 1; i++)
			{
				if (i > 0 && i <= _imgMetaList.Count - 1)
				{
					if (i % 3 == 0)
					{
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i - 3]);
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i + 1]);
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i + 3]);
					}
					else if (i % 3 == 1)
					{
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i - 3]);
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i - 1]);
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i + 1]);
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i + 3]);
					}
					else if (i % 3 == 2)
					{
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i - 3]);
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i - 1]);
						_imgMetaList[i].Neightbours.Add(_imgMetaList[i + 3]);
					}
				}
			}
			return _imgMetaList;
		}


		private List<ImageMetaOutput> Shuffle()
		{
			for (int i = 0; i < _imgMetaList.Count - 1; i++)
			{
				for (int j = i + 1; j < _imgMetaList.Count; j++)
				{


				}
			}

			return _imgMetaList;
		}
	}
}
