using ImageObjDetection.API.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageObjDetection.API.Middleware
{
	public class ApiKeyValidatorMiddleware
	{
		private readonly RequestDelegate _next;

		public ApiKeyValidatorMiddleware(RequestDelegate next)
		{
			_next = next;
		}


		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				if (httpContext.Request.Headers.Keys.Contains("Access-Control-Request-Headers") || httpContext.Request.Headers.Keys.Contains("access-control-request-headers"))
				{
					string[] key;
					key = httpContext.Request.Headers.Where(c => c.Key == "Access-Control-Request-Headers" || c.Key == "access-control-request-headers")
						.FirstOrDefault().Value[0].Split(",");//.Query;

					if (String.IsNullOrEmpty(key[0]))
					{
						httpContext.Response.StatusCode = 400; //Bad Request                
						await httpContext.Response.WriteAsync("API Key is missing");
						return;
					}
					else
					{
						string[] serviceName = httpContext.Request.Path.Value.Split('/');

						if (!ReturnApiKey(key[0]))
						{
							httpContext.Response.StatusCode = 401; //UnAuthorized
							await httpContext.Response.WriteAsync("Invalid User Key or Request");
							return;
						}

					}

					await _next.Invoke(httpContext);

				}
				else
				{
					httpContext.Response.StatusCode = 400; //Bad Request                
					await httpContext.Response.WriteAsync("API Key is missing");
					return;
				}

			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				throw;
			}
		}


		public Boolean ReturnApiKey(string apiKey)
		{
			ApiKeys apiKeys = new ApiKeys();
			Dictionary<string, string> apiKeyList = apiKeys.DicApiKey();
			if (!string.IsNullOrEmpty(apiKey))
			{
				string value = "";
				return apiKeyList.TryGetValue(apiKey, out value);
			}
			else
				return false;
		}
	}
}
