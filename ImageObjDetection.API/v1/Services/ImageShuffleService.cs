using ImageObjDetection.API.Models;
using OnnxObjectDetection;
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
			AddNeighbours2();
			Shuffle();

			var fileNames = _imgMetaList.OrderBy(x => x.Id).Select(x => x.FileName).ToList();


			return fileNames;
		}

		private List<ImageMetaOutput> UnionFilteredBoxes()
		{
			foreach (var node in _imgMetaList)
			{
				if (node.FilteredBoxes.Count > 0)
				{
					float xMin = node.FilteredBoxes.Min(s => s.Dimensions.X);
					float yMin = node.FilteredBoxes.Min(s => s.Dimensions.Y);
					float xMax = node.FilteredBoxes.Max(s => s.Dimensions.X + s.Dimensions.Width);
					float yMax = node.FilteredBoxes.Max(s => s.Dimensions.Y + s.Dimensions.Height);

					xMin = (uint)node.ImgSize.Width * xMin / ImageSettings.imageWidth;
					yMin = (uint)node.ImgSize.Height * yMin / ImageSettings.imageHeight;
					xMax = (uint)node.ImgSize.Width * xMax / ImageSettings.imageWidth;
					yMax = (uint)node.ImgSize.Height * yMax / ImageSettings.imageHeight;

					node.UnionBox = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);
				}
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
						node.BoxOrientations.Add(Orientation.Left);
					if (node.UnionBox.Right >= node.ImgSize.Width - cutoff)
						node.BoxOrientations.Add(Orientation.Right);
					if (node.UnionBox.Top <= 0)
						node.BoxOrientations.Add(Orientation.Top);
					if (node.UnionBox.Bottom >= node.ImgSize.Height)
						node.BoxOrientations.Add(Orientation.Bottom);
				}
				else
				{
					cutoff = (node.ImgSize.Height - node.ImgSize.Width) / 2;
					if (node.UnionBox.Left <= 0)
						node.BoxOrientations.Add(Orientation.Left);
					if (node.UnionBox.Right >= node.ImgSize.Width)
						node.BoxOrientations.Add(Orientation.Right);
					if (node.UnionBox.Top <= cutoff)
						node.BoxOrientations.Add(Orientation.Top);
					if (node.UnionBox.Bottom >= node.ImgSize.Height - cutoff)
						node.BoxOrientations.Add(Orientation.Bottom);
				}
			}
			return _imgMetaList;
		}



		private List<ImageMetaOutput> AddNeighbours2()
		{
			for (int i = 0; i < _imgMetaList.Count - 1; i++)
			{
				if (i % 3 == 0)
				{
					if (i >= 3)
						_imgMetaList[i].Neightbours.Add(Orientation.Top, _imgMetaList[i - 3]);
					if (i <= _imgMetaList.Count - 2)
						_imgMetaList[i].Neightbours.Add(Orientation.Right, _imgMetaList[i + 1]);
					if (i <= _imgMetaList.Count - 4)
						_imgMetaList[i].Neightbours.Add(Orientation.Bottom, _imgMetaList[i + 3]);
				}
				else if (i % 3 == 1)
				{
					if (i >= 3)
						_imgMetaList[i].Neightbours.Add(Orientation.Top, _imgMetaList[i - 3]);
					if (i >= 1)
						_imgMetaList[i].Neightbours.Add(Orientation.Left, _imgMetaList[i - 1]);
					if (i <= _imgMetaList.Count - 2)
						_imgMetaList[i].Neightbours.Add(Orientation.Right, _imgMetaList[i + 1]);
					if (i <= _imgMetaList.Count - 4)
						_imgMetaList[i].Neightbours.Add(Orientation.Bottom, _imgMetaList[i + 3]);
				}
				else if (i % 3 == 2)
				{
					if (i >= 3)
						_imgMetaList[i].Neightbours.Add(Orientation.Top, _imgMetaList[i - 3]);
					if (i >= 1)
						_imgMetaList[i].Neightbours.Add(Orientation.Left, _imgMetaList[i - 1]);
					if (i <= _imgMetaList.Count - 4)
						_imgMetaList[i].Neightbours.Add(Orientation.Bottom, _imgMetaList[i + 3]);
				}
			}
			return _imgMetaList;
		}



		private List<ImageMetaOutput> Shuffle()
		{
			for (int i = 0; i < _imgMetaList.Count - 1; i++)
			{
				FindCollisions(_imgMetaList[i]);

				if (_imgMetaList[i].BoxCollisions.Count > 0)
				{
					for (int j = i + 1; j < _imgMetaList.Count; j++)
					{
						var intersect = _imgMetaList[i].BoxCollisions.Intersect(_imgMetaList[j].BoxOrientations);
						if (intersect.Count() == 0)
						{
							var tempId = _imgMetaList[i].Id;
							var tempNeighbours = _imgMetaList[i].Neightbours;
							_imgMetaList[i].Id = _imgMetaList[j].Id;
							_imgMetaList[i].Neightbours = _imgMetaList[j].Neightbours;

							_imgMetaList[j].Id = tempId;
							_imgMetaList[j].Neightbours = tempNeighbours;

							FindCollisions(_imgMetaList[i]);
							FindCollisions(_imgMetaList[j]);
							break;
						}
					}
				}
			}

			return _imgMetaList;
		}

		private void FindCollisions(ImageMetaOutput node)
		{
			foreach (var orientation in node.BoxOrientations)
			{
				switch (orientation)
				{
					case Orientation.Bottom:
						{
							node.Neightbours.TryGetValue(Orientation.Bottom, out ImageMetaOutput neighbour);
							if (neighbour != null && neighbour.BoxOrientations.Contains(Orientation.Top) && neighbour.Id > node.Id)
							{
								node.BoxCollisions.Add(Orientation.Bottom);
							}
							break;
						}

					case Orientation.Top:
						{
							node.Neightbours.TryGetValue(Orientation.Top, out ImageMetaOutput neighbour);
							if (neighbour != null && neighbour.BoxOrientations.Contains(Orientation.Bottom) && neighbour.Id < node.Id)
							{
								node.BoxCollisions.Add(Orientation.Top);
							}
							break;
						}

					case Orientation.Right:
						{
							node.Neightbours.TryGetValue(Orientation.Right, out ImageMetaOutput neighbour);
							if (neighbour != null && neighbour.BoxOrientations.Contains(Orientation.Left) && neighbour.Id > node.Id)
							{
								node.BoxCollisions.Add(Orientation.Right);
							}
							break;
						}

					case Orientation.Left:
						{
							node.Neightbours.TryGetValue(Orientation.Left, out ImageMetaOutput neighbour);
							if (neighbour != null && neighbour.BoxOrientations.Contains(Orientation.Right) && neighbour.Id < node.Id)
							{
								node.BoxCollisions.Add(Orientation.Left);
							}
							break;
						}
				}
			}
		}
	}
}
