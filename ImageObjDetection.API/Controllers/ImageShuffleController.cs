using ImageObjDetection.API.v1.Dtos;
using ImageObjDetection.API.v1.Services;
using Microsoft.AspNetCore.Mvc;
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

		public ImageShuffleController(ILogger<ImageShuffleController> logger)
		{
			_firebaseStorageService = new FirebaseStorageService();
			_logger = logger;
		}

		[HttpPost]
		public async Task<ActionResult<string>> PostProcessData([FromBody] UserData value)
		{
			try
			{
				if (value != null)
				{
					var accessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImUwOGI0NzM0YjYxNmE0MWFhZmE5MmNlZTVjYzg3Yjc2MmRmNjRmYTIiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiTGVvbmlkIE1hbGFub3dza2kiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EtL0FPaDE0R2gtdTdXaUNLMG5MSjBFTXFNN3ZrRVNBVWQ4MXBDX1dma0h1eHNWPXM5Ni1jIiwiaXNzIjoiaHR0cHM6Ly9zZWN1cmV0b2tlbi5nb29nbGUuY29tL2ltYWdlLXNodWZmbGVyLXVpIiwiYXVkIjoiaW1hZ2Utc2h1ZmZsZXItdWkiLCJhdXRoX3RpbWUiOjE2MDk3MDA3MjksInVzZXJfaWQiOiJQMTl4SjBOODMyUUtGcVFNRk1xTGRSMmRma1EyIiwic3ViIjoiUDE5eEowTjgzMlFLRnFRTUZNcUxkUjJkZmtRMiIsImlhdCI6MTYwOTg5Nzg3MywiZXhwIjoxNjA5OTAxNDczLCJlbWFpbCI6ImZhbGF0cm9uMkBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJnb29nbGUuY29tIjpbIjEwNDk0MzY1ODA0Mzg5MjQwNDE5NyJdLCJlbWFpbCI6WyJmYWxhdHJvbjJAZ21haWwuY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoiZ29vZ2xlLmNvbSJ9fQ.4kcPVP44HuhxE4fVtimBdfAZXNHH14NqxYjD1i7TZkclMr0vhNxBVpnn0GbS45weUie00J115IPS-mcp7PO3axQl4uL0bxhFFkYjmsti4utH0rTIPLVToZatMbWRm6dO_e0nm5A6-TrHzJ6b6vRK1sL7lEhp3c16VJxWlzwdFi9NwQmuCPqnO57rrpifG1-Z9BNBGz1BEA6iLz2XecsMqAS1PctJh75ISR4Iqmrh-Mz0wahI0jhqeEKM9N-qKNBL-87_vVB8WkihXBP5jHC7c9Xgh3o1DkJvmyN-MJg6psrgHHaF1U-J74mAPIU9w-Wrmrzwi09hnNJhpisH7Drn3Q";

					await _firebaseStorageService.ProcessData(value, accessToken);

					return Ok();
					//return CreatedAtAction("PostProcessData", submittalTask.StatusId);
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
