using Firebase.Auth;
using Firebase.Storage;
using ImageObjDetection.API.Models;
using ImageObjDetection.API.v1.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OnnxObjectDetection;
using OnnxObjectDetection.Service.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageObjDetection.API.v1.Services
{
	public class FirebaseStorageService
	{
		private string apiKey = "";
		private string bucket = "image-shuffler-ui.appspot.com";
		private string authEmail = "";
		private string password = "";

		private string base64String = string.Empty;
		//private readonly string _imagesTmpFolder;

		private readonly ILogger<FirebaseStorageService> _logger;
		private readonly IObjectDetectionService _objectDetectionService;

		public FirebaseStorageService(IConfiguration Configuration, ILogger<FirebaseStorageService> logger, IObjectDetectionService objectDetectionService)
		{
			authEmail = Configuration["environmentVariables:Firebase_AuthEmail"];
			password = Configuration["environmentVariables:Firebase_Password"];
			apiKey = Configuration["environmentVariables:Firebase_ApiKey"];

			_logger = logger;
			_objectDetectionService = objectDetectionService;
		}

		public class Result
		{
			public string imageString { get; set; }
		}




		public async Task<List<string>> ProcessData(UserData userData)
		{
			FirebaseStorage firebase = await CreateFirebaseReferenceAsync();

			List<ImageMetaOutput> imgMetaList = new List<ImageMetaOutput>();
			List<string> sortedFileNames = null;

			for (int i = 0; i < userData.FileNames.Length; i++)
			{
				ImageMetaOutput imgMetaOutput = null;
				MemoryStream stream = await DownloadFileFromUrl(userData.UserEmail, userData.DateTime, userData.FileNames[i], firebase);
				var outputStream = IdentifyObjects(stream, ref imgMetaOutput);
				imgMetaOutput.FileName = userData.FileNames[i];

				UploadFile(outputStream, userData.UserEmail, userData.DateTime, userData.FileNames[i], firebase);
				imgMetaList.Add(imgMetaOutput);
			}

			ImageShuffleService imageShuffleService = new ImageShuffleService(imgMetaList);
			sortedFileNames = imageShuffleService.Solve();



			return sortedFileNames;
		}



		public async Task<MemoryStream> DownloadFileFromUrl(string userEmail, string dateTime, string fileName, FirebaseStorage firebase)
		{
			var imageUrl = await firebase
			  .Child("images")
			  .Child(userEmail)
			  .Child(dateTime)
			  .Child(fileName)
			  .GetDownloadUrlAsync();


			MemoryStream stream = null;
			using (var client = new HttpClient())
			{
				//client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				var content = await client.GetByteArrayAsync(imageUrl);
				stream = new MemoryStream(content);
				return stream;
			}
		}


		public async void UploadFile(MemoryStream memoryStream, string userEmail, string dateTime, string fileName, FirebaseStorage firebase)
		{
			// Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
			Console.Write(memoryStream.Length);
			using (memoryStream)
			{
				// Constructr FirebaseStorage, path to where you want to upload the file and Put it there
				var task = firebase
				.Child("images")
				.Child(userEmail)
				.Child(dateTime)
				.Child("objectsDetected")
				.Child(fileName)
				.PutAsync(memoryStream);

				// Track progress of the upload
				task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

				// await the task to wait until upload completes and get the download url
				var downloadUrl = await task;
			}
		}

		private async Task<FirebaseStorage> CreateFirebaseReferenceAsync()
		{
			var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
			var auth = await authProvider.SignInWithEmailAndPasswordAsync(authEmail, password);

			var firebase = new FirebaseStorage(bucket,
			  new FirebaseStorageOptions
			  {
				  AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
			  });
			return firebase;
		}


		public MemoryStream IdentifyObjects(MemoryStream imageMemoryStream, ref ImageMetaOutput imageMetaOutput)
		{
			try
			{
				//Convert to Image
				Image image = Image.FromStream(imageMemoryStream);

				//Convert to Bitmap
				Bitmap bitmapImage = (Bitmap)image;

				_logger.LogInformation($"Start processing image...");

				//Measure execution time
				var watch = System.Diagnostics.Stopwatch.StartNew();

				//Set the specific image data into the ImageInputData type used in the DataView
				ImageInputData imageInputData = new ImageInputData { Image = bitmapImage };

				//Detect the objects in the image                
				var result = DetectAndPaintImage(imageInputData, imageMemoryStream, ref imageMetaOutput);

				//Stop measuring time
				watch.Stop();
				var elapsedMs = watch.ElapsedMilliseconds;
				_logger.LogInformation($"Image processed in {elapsedMs} miliseconds");
				return result;
			}
			catch (Exception e)
			{
				_logger.LogInformation("Error is: " + e.Message);
				return null;
			}
		}

		private MemoryStream DetectAndPaintImage(ImageInputData imageInputData, MemoryStream imageMemoryStream, ref ImageMetaOutput imageMetaOutput)
		{
			//Predict the objects in the image
			_objectDetectionService.DetectObjectsUsingModel(imageInputData);
			var img = _objectDetectionService.DrawBoundingBox(imageMemoryStream);

			MemoryStream m = new MemoryStream();
			img.Save(m, img.RawFormat);
			m.Position = 0;

			imageMetaOutput = new ImageMetaOutput
			{
				FilteredBoxes = ((ObjectDetectionService)_objectDetectionService).FilteredBoxes,
				ImgSize = img.Size,
			};


			return m;
		}


	}
}
