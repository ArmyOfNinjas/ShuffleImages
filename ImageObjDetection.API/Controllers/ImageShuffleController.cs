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
					var accessToken = "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6ImUwOGI0NzM0YjYxNmE0MWFhZmE5MmNlZTVjYzg3Yjc2MmRmNjRmYTIiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiTGVvbmlkIE1hbGFub3dza2kiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EtL0FPaDE0R2gtdTdXaUNLMG5MSjBFTXFNN3ZrRVNBVWQ4MXBDX1dma0h1eHNWPXM5Ni1jIiwiaXNzIjoiaHR0cHM6Ly9zZWN1cmV0b2tlbi5nb29nbGUuY29tL2ltYWdlLXNodWZmbGVyLXVpIiwiYXVkIjoiaW1hZ2Utc2h1ZmZsZXItdWkiLCJhdXRoX3RpbWUiOjE2MDg5NTkzMzQsInVzZXJfaWQiOiJQMTl4SjBOODMyUUtGcVFNRk1xTGRSMmRma1EyIiwic3ViIjoiUDE5eEowTjgzMlFLRnFRTUZNcUxkUjJkZmtRMiIsImlhdCI6MTYwOTkwMjU3MywiZXhwIjoxNjA5OTA2MTczLCJlbWFpbCI6ImZhbGF0cm9uMkBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJnb29nbGUuY29tIjpbIjEwNDk0MzY1ODA0Mzg5MjQwNDE5NyJdLCJlbWFpbCI6WyJmYWxhdHJvbjJAZ21haWwuY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoiZ29vZ2xlLmNvbSJ9fQ.DstfByrVsfrUBJJLdYHVclv90tFwAjLNA7L3wCJVrj5h_rwxL1-TmPpcuTjW83hHNdt40YSkQCkItb5nR1HcO-EOjb9AjmWA9Sy4TCqkOinEd06nC7H--Lan6ug8jSg5bjgMe8HWldLLMqklou5sNAqthJKADYFlPyECESEZu1M7yOMYIXPJ_ixr284W2NFGlyvV1oJ6LIFFWj7Dikg2eOaXJWraGYBIdHJhAJ0sOCkvOe55WOz2emDZGTBVDmhi7xdjyOD17uLJO92gCOAfMiQZTufsB819SNfupMzS5Ejst7sRRu_T0OAgFSOwuX5YbKcQx1NIZhK068Tk26_1dg";

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
