using ImageObjDetection.API.Models;
using System;
using System.Collections.Generic;
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
		//		 0 1 2	
				 
		//	0//	 0 1 2			0.0		0.1		0.2
		//	1//	 3 4 5			1.0		1.1		1.2
		//	2//	 6 7 8			2.0		2.1		2.2






			return new List<string>();
		}
	}
}
