using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Microsoft.ML;
using ObjectRecognitionONNX.YoloParser;
using ObjectRecognitionONNX.DataStructures;
using ObjectRecognitionONNX;
using ObjectRecognitionNNOX;

namespace ObjectRecognitionONNX
{
	public class ONNXObjectRecognizer
	{
		//public MemoryStream DetectObjects(MemoryStream stream)
		//{
		//	MemoryStream updatedStream = DetectObject(stream);
		//	return updatedStream;
		//}


		//private MemoryStream DetectObject(MemoryStream stream)
		//{
		//	MemoryStream updatedStream = null;
		//	var configurationDetector = new ConfigurationDetector();
		//	var config = configurationDetector.Detect();
		//	var yolo = new YoloWrapper(config);
		//	//var memoryStream = new MemoryStream();
		//	//picImage.Image.Save(memoryStream, ImageFormat.Png);
		//	using (stream)
		//	{
		//		var items = yolo.Detect(stream.ToArray()).ToList();
		//		updatedStream = AddDetailsToPictureBox(stream, items);
		//	}
		//	return updatedStream;
		//}

		//private MemoryStream AddDetailsToPictureBox(MemoryStream stream, List<YoloItem> items)
		//{
		//	Image image = Image.FromStream(stream);
		//	var font = new Font("Arial", 15, FontStyle.Bold);
		//	var brush = new SolidBrush(Color.Red);
		//	var graphics = Graphics.FromImage(image);

		//	foreach (var item in items)
		//	{
		//		var x = item.X;
		//		var y = item.Y;
		//		var width = item.Width;
		//		var height = item.Height;

		//		var rect = new Rectangle(x, y, width, height);
		//		var pen = new Pen(Color.Red, 2);

		//		graphics.DrawRectangle(pen, rect);
		//		graphics.DrawString(item.Type, font, brush, new Point(x, y));
		//	}

		//	MemoryStream updatedStream = new MemoryStream();
		//	image.Save(updatedStream, System.Drawing.Imaging.ImageFormat.Jpeg);

		//	return updatedStream;
		//}








		public MemoryStream DetectObjects(MemoryStream stream)
		{
			var assetsRelativePath = @"../../../assets";
			string dir = Directory.GetCurrentDirectory().Replace("ImageObjDetection.API","ObjectRecognitionONNX");
			string assetsPath = GetAbsolutePath(assetsRelativePath);
			string assetsPath2 = @"C:\GitHub\ArmyOfNinjas\ShuffleImages\ObjectRecognitionNNOX\bin\Debug\netcoreapp3.1\assets";
			//string assetsPath2 = Path.Combine(dir, "assets");
			var modelFilePath = Path.Combine(assetsPath2, "Model", "TinyYolo2_model.onnx");
			var imagesFolder = Path.Combine(assetsPath2, "images");
			var outputFolder = Path.Combine(assetsPath2, "images", "output");


			MemoryStream updatedStream = null;
			MLContext mlContext = new MLContext();

			try
			{
				Bitmap bitmapImage = (Bitmap)Image.FromStream(stream);
				IEnumerable<ImageInputData> images = new List<ImageInputData>() { new ImageInputData { Image = bitmapImage } };
				IDataView imageDataView = mlContext.Data.LoadFromEnumerable(images);

				//IEnumerable<ImageNetData> images = ImageNetData.ReadFromFile(imagesFolder);
				//IDataView imageDataView = mlContext.Data.LoadFromEnumerable(images);
				var modelScorer = new OnnxModelScorer(imagesFolder, modelFilePath, mlContext);

				// Use model to score data
				IEnumerable<float[]> probabilities = modelScorer.Score(imageDataView);
				YoloOutputParser parser = new YoloOutputParser();

				var boundingBoxes =
					probabilities
					.Select(probability => parser.ParseOutputs(probability))
					.Select(boxes => parser.FilterBoundingBoxes(boxes, 5, .5F));


				//for (var i = 0; i < images.Count(); i++)
				//{
				//	string imageFileName = images.ElementAt(i).Label;
				//	IList<YoloBoundingBox> detectedObjects = boundingBoxes.ElementAt(i);
				//	DrawBoundingBox(imagesFolder, outputFolder, imageFileName, detectedObjects);
				//	LogDetectedObjects(imageFileName, detectedObjects);
				//}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			Console.WriteLine("========= End of Process..Hit any Key ========");
			Console.ReadLine();
			return updatedStream;
		}

		public static string GetAbsolutePath(string relativePath)
		{
			FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
			string assemblyFolderPath = _dataRoot.Directory.FullName;

			string fullPath = Path.Combine(assemblyFolderPath, relativePath);

			return fullPath;
		}

		private static void DrawBoundingBox(string inputImageLocation, string outputImageLocation, string imageName, IList<YoloBoundingBox> filteredBoundingBoxes)
		{
			Image image = Image.FromFile(Path.Combine(inputImageLocation, imageName));

			var originalImageHeight = image.Height;
			var originalImageWidth = image.Width;

			foreach (var box in filteredBoundingBoxes)
			{
				var x = (uint)Math.Max(box.Dimensions.X, 0);
				var y = (uint)Math.Max(box.Dimensions.Y, 0);
				var width = (uint)Math.Min(originalImageWidth - x, box.Dimensions.Width);
				var height = (uint)Math.Min(originalImageHeight - y, box.Dimensions.Height);

				x = (uint)originalImageWidth * x / OnnxModelScorer.ImageNetSettings.imageWidth;
				y = (uint)originalImageHeight * y / OnnxModelScorer.ImageNetSettings.imageHeight;
				width = (uint)originalImageWidth * width / OnnxModelScorer.ImageNetSettings.imageWidth;
				height = (uint)originalImageHeight * height / OnnxModelScorer.ImageNetSettings.imageHeight;

				string text = $"{box.Label} ({(box.Confidence * 100).ToString("0")}%)";

				using (Graphics thumbnailGraphic = Graphics.FromImage(image))
				{
					thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
					thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
					thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

					// Define Text Options
					Font drawFont = new Font("Arial", 12, FontStyle.Bold);
					SizeF size = thumbnailGraphic.MeasureString(text, drawFont);
					SolidBrush fontBrush = new SolidBrush(Color.Black);
					Point atPoint = new Point((int)x, (int)y - (int)size.Height - 1);

					// Define BoundingBox options
					Pen pen = new Pen(box.BoxColor, 3.2f);
					SolidBrush colorBrush = new SolidBrush(box.BoxColor);

					thumbnailGraphic.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);
					thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint);

					// Draw bounding box on image
					thumbnailGraphic.DrawRectangle(pen, x, y, width, height);

				}
			}

			if (!Directory.Exists(outputImageLocation))
			{
				Directory.CreateDirectory(outputImageLocation);
			}

			image.Save(Path.Combine(outputImageLocation, imageName));
		}

		private static void LogDetectedObjects(string imageName, IList<YoloBoundingBox> boundingBoxes)
		{
			Console.WriteLine($".....The objects in the image {imageName} are detected as below....");

			foreach (var box in boundingBoxes)
			{
				Console.WriteLine($"{box.Label} and its Confidence score: {box.Confidence}");
			}

			Console.WriteLine("");
		}
	}
}
