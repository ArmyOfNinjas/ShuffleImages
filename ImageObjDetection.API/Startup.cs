using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using OnnxObjectDetection;
using OnnxObjectDetection.API.Utilities;
using OnnxObjectDetection.Service.Infrastructure;
using OnnxObjectDetection.Service.Services;

namespace ImageObjDetection.API
{
	public class Startup
	{
		readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
		private readonly string _onnxModelFilePath;
		private readonly string _mlnetModelFilePath;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;

			_onnxModelFilePath = CommonHelpers.GetAbsolutePath(Configuration["MLModel:OnnxModelFilePath"]);
			_mlnetModelFilePath = CommonHelpers.GetAbsolutePath(Configuration["MLModel:MLNETModelFilePath"]);
			var onnxModelConfigurator = new OnnxModelConfigurator(new TinyYoloModel(_onnxModelFilePath));
			onnxModelConfigurator.SaveMLNetModel(_mlnetModelFilePath);
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddControllers();
			services.AddCors(options =>
			{
				options.AddPolicy(name: MyAllowSpecificOrigins,
								  builder =>
								  {
									  builder.WithOrigins("http://localhost:3000/");
								  });
			});

			//_moviesApiKey = Configuration["Movies:ServiceApiKey"];

			services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

			services.AddPredictionEnginePool<ImageInputData, TinyYoloPrediction>().FromFile(_mlnetModelFilePath);

			services.AddTransient<IImageFileWriter, ImageFileWriter>();
			services.AddTransient<IObjectDetectionService, ObjectDetectionService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors(MyAllowSpecificOrigins);

			app.UseAuthorization();

			//app.UseMiddleware<ApiKeyValidatorMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
