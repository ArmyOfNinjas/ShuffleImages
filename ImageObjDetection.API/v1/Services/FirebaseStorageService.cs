using Firebase.Auth;
using Firebase.Storage;
using ImageObjDetection.API.v1.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using OnnxObjectDetection;
using OnnxObjectDetectionWeb.Services;
using OnnxObjectDetectionWeb.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly string _imagesTmpFolder;

        private readonly ILogger<FirebaseStorageService> _logger;
        private readonly IObjectDetectionService _objectDetectionService;

        public FirebaseStorageService(IConfiguration Configuration, ILogger<FirebaseStorageService> logger, IObjectDetectionService objectDetectionService)
        {
            authEmail = Configuration["environmentVariables:Firebase_AuthEmail"];
            password = Configuration["environmentVariables:Firebase_Password"];
            apiKey = Configuration["environmentVariables:Firebase_ApiKey"];

            _logger = logger;
            _objectDetectionService = objectDetectionService;
            _imagesTmpFolder = CommonHelpers.GetAbsolutePath(@"../../../ImagesTemp");
        }

        public class Result
        {
            public string imageString { get; set; }
        }




        public async Task<List<MemoryStream>> ProcessData(UserData userData)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(authEmail, password);

            List<MemoryStream> streams = new List<MemoryStream>();
            for (int i = 0; i < userData.FileNames.Length; i++)
            {
                MemoryStream stream = await DownloadFileFromUrl(userData.UserEmail, userData.DateTime, userData.FileNames[i], auth);
                var updatedStream = IdentifyObjects(stream);

                UploadFile(updatedStream, userData.UserEmail, userData.DateTime, userData.FileNames[i], auth);
                streams.Add(updatedStream);
            }

            return streams;
        }



        public async Task<MemoryStream> DownloadFileFromUrl(string userEmail, string dateTime, string fileName, FirebaseAuthLink auth)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            //var auth = await authProvider.SignInWithEmailAndPasswordAsync(authEmail, password);

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
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = await client.GetByteArrayAsync(imageUrl);
                stream = new MemoryStream(content);
                return stream;
            }
        }


        public async void UploadFile(MemoryStream memoryStream, string userEmail, string dateTime, string fileName, FirebaseAuthLink auth)
        {
            // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
            //string assetsPath = GetAbsolutePath(@"../../../v1/Images/1.jpg");
            //var stream = File.Open(assetsPath, FileMode.Open);

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            //var auth = await authProvider.SignInWithEmailAndPasswordAsync(authEmail, password);

            var firebase = new FirebaseStorage(bucket,
              new FirebaseStorageOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
              });

            Console.Write(memoryStream.Length);
            //using (memoryStream)
            //{
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
            //}
        }



        public MemoryStream IdentifyObjects(MemoryStream imageMemoryStream)
        {
            try
            {
                //Convert to Image
                Image image = Image.FromStream(imageMemoryStream);

                string fileName = string.Format("{0}.Jpeg", image.GetHashCode());
                string imageFilePath = Path.Combine(_imagesTmpFolder, fileName);
                //save image to a path
                image.Save(imageFilePath, ImageFormat.Jpeg);

                //Convert to Bitmap
                Bitmap bitmapImage = (Bitmap)image;

                _logger.LogInformation($"Start processing image...");

                //Measure execution time
                var watch = System.Diagnostics.Stopwatch.StartNew();

                //Set the specific image data into the ImageInputData type used in the DataView
                ImageInputData imageInputData = new ImageInputData { Image = bitmapImage };

                //Detect the objects in the image                
                var result = DetectAndPaintImage(imageInputData, imageFilePath);

                //Stop measuring time
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                _logger.LogInformation($"Image processed in {elapsedMs} miliseconds");
                return result;
                //return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error is: " + e.Message);
                return null;
                //return BadRequest();
            }
        }

        private MemoryStream DetectAndPaintImage(ImageInputData imageInputData, string imageFilePath)
        {
            //Predict the objects in the image
            _objectDetectionService.DetectObjectsUsingModel(imageInputData);
            var img = _objectDetectionService.DrawBoundingBox(imageFilePath);

            MemoryStream m = new MemoryStream();
            img.Save(m, img.RawFormat);
            return m;
            //using (MemoryStream m = new MemoryStream())
            //{
            //    img.Save(m, img.RawFormat);
            //    byte[] imageBytes = m.ToArray();

            //    // Convert byte[] to Base64 String
            //    base64String = Convert.ToBase64String(imageBytes);
            //    var result = new Result { imageString = base64String };
            //    return m;
            //}
        }








    }
}
