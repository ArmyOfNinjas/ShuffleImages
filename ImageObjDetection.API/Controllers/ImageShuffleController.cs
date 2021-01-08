using ImageObjDetection.API.v1.Dtos;
using ImageObjDetection.API.v1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageObjDetection.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ImageShuffleController : Controller
	{

		private readonly ILogger<ImageShuffleController> _logger;
		private readonly FirebaseStorageService _firebaseStorageService;

		public ImageShuffleController(ILogger<ImageShuffleController> logger, IConfiguration Configuration)
		{
			_firebaseStorageService = new FirebaseStorageService(Configuration);
			_logger = logger;
		}

		[HttpPost]
		public async Task<ActionResult<string>> PostProcessData([FromBody] UserData value)
		{
			try
			{
				if (value != null)
				{
					var accessToken = "";

					var streams = await _firebaseStorageService.ProcessData(value, accessToken);

					return Ok();
				}
				else return BadRequest("received null");

			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
	}
}
