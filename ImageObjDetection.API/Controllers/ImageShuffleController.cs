﻿using ImageObjDetection.API.v1.Dtos;
using ImageObjDetection.API.v1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OnnxObjectDetectionWeb.Services;
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

		private readonly FirebaseStorageService _firebaseStorageService;

		public ImageShuffleController(ILogger<FirebaseStorageService> logger, IConfiguration Configuration, IObjectDetectionService objectDetectionService)
		{
			_firebaseStorageService = new FirebaseStorageService(Configuration, logger, objectDetectionService);
		}

		[HttpPost]
		public async Task<ActionResult<string>> PostProcessData([FromBody] UserData value)
		{
			try
			{
				if (value != null)
				{
					var streams = await _firebaseStorageService.ProcessData(value);

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
