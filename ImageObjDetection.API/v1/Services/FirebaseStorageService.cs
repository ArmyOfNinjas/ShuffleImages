using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageObjDetection.API.v1.Services
{
    public class FirebaseStorageService
    {
        private string apiKey = "AIzaSyAnjoXZd7IXXE0Y3N3W1urGBCkuy_MD1SI";
        private string bucket = "image-shuffler-ui.appspot.com/";
        private string authEmail = "api-image-shuffler@gmail.com";
        private string password = "Shuffle12Api!@";

        public async void UploadFile(string userEmail, string date)
        {
            // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
            var stream = File.Open(@"C:\Users\you\file.png", FileMode.Open);

            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(bucket)
                .Child("images")
                .Child("random")
                .Child("file.png")
                .PutAsync(stream);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;
        }

        public async Task<MemoryStream>  DownloadFile(string userEmail, string date, string url)
        {
            MemoryStream stream;
            using (var client = new HttpClient())
            {
                var content = await client.GetByteArrayAsync(url);
                stream = new MemoryStream(content);
                return stream;
                //using (var stream = new MemoryStream(content))
                //{

                //}
            }
        }


    }
}
