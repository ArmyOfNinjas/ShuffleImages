using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageObjDetection.API.v1.Dtos
{
	public class UserData
	{
		public string UserEmail { get; set; }
		public string[] URLs { get; set; }
		public string DateTime { get; set; }
	}
}
