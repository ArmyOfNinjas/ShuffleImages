using Firebase.Auth;
using Firebase.Storage;
using ImageObjDetection.API.v1.Dtos;
using ImageObjDetectionForm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ImageObjDetection.API.v1.Services
{
	public class FirebaseStorageService
	{
		private string apiKey = "AIzaSyAnjoXZd7IXXE0Y3N3W1urGBCkuy_MD1SI";
		private string bucket = "image-shuffler-ui.appspot.com";
		private string authEmail = "api-image-shuffler@gmail.com";
		private string password = "Shuffle12Api!@";

		public async void UploadFile(MemoryStream memoryStream, string userEmail, string dateTime, string fileName)
		{
			// Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
			//string assetsPath = GetAbsolutePath(@"../../../v1/Images/1.jpg");
			//var stream = File.Open(assetsPath, FileMode.Open);

			var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
			var auth = await authProvider.SignInWithEmailAndPasswordAsync(authEmail, password);

			var firebase = new FirebaseStorage(bucket,
			  new FirebaseStorageOptions
			  {
				  AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
			  });

			// Constructr FirebaseStorage, path to where you want to upload the file and Put it there
			var task = firebase
				.Child("images")
				.Child(userEmail)
				.Child(dateTime)
				.Child("detected objects")
				.Child(fileName)
				.PutAsync(memoryStream);

			// Track progress of the upload
			task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

			// await the task to wait until upload completes and get the download url
			var downloadUrl = await task;
		}


		public async Task<List<MemoryStream>> ProcessData(UserData userData, string accessToken)
		{
			List<MemoryStream> streams = new List<MemoryStream>();
			YoloObjectDetector yoloObjectDetector = new YoloObjectDetector();
			for (int i = 0; i < userData.FileNames.Length; i++)
			{
				MemoryStream stream = await DownloadFileFromUrl(userData.UserEmail, userData.DateTime, userData.FileNames[i], accessToken);
				MemoryStream updatedStream = yoloObjectDetector.DetectObjects(stream);
				UploadFile(updatedStream, userData.UserEmail, userData.DateTime, userData.FileNames[i]);
				streams.Add(updatedStream);

				using (updatedStream)
				{
					var modelsRelativePath = @"../../../v1/Images";
					string assetsPath = GetAbsolutePath(modelsRelativePath);
					using (FileStream fs = new FileStream($"{assetsPath}/{userData.FileNames[i]}", FileMode.OpenOrCreate))
					{
						updatedStream.CopyTo(fs);
						fs.Flush();
					}
				}
			}

			return streams;
		}




		public async Task<MemoryStream> DownloadFileFromUrl(string userEmail, string dateTime, string fileName, string accessToken)
		{
			//var auth = await authProvider.SignInWithCustomTokenAsync(accessToken);
			var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
			//var auth = await authProvider.SignInWithEmailAndPasswordAsync(authEmail, password);
			var auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google, accessToken);

			var firebase = new FirebaseStorage(bucket,
			  new FirebaseStorageOptions
			  {
				  AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
			  });

			var imageUrl = await firebase
			  .Child("images")
			  .Child(userEmail)
			  .Child(dateTime)
			  .Child(fileName)
			  .GetDownloadUrlAsync();


			MemoryStream stream = null;
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				var content = await client.GetByteArrayAsync(imageUrl);
				stream = new MemoryStream(content);
				return stream;
			}
		}


		public static string GetAbsolutePath(string relativePath)
		{
			FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
			string assemblyFolderPath = _dataRoot.Directory.FullName;

			string fullPath = Path.Combine(assemblyFolderPath, relativePath);

			return fullPath;
		}

	}
}
