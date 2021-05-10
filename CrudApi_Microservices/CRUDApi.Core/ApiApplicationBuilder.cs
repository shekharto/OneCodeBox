using CRUD.Transaction.CRUDApi.Core.Extensions;
using CRUD.Transaction.CRUDApi.Core.Interface;
using CRUD.Transaction.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace CRUD.Transaction.CRUDApi.Core
{
	public class ApiApplicationBuilder
	{
		private readonly string _CorsPolicy = "AllowCorsPolicy";
		private readonly string[] _allowedMethods = new string[4] { "GET", "POST", "PATCH", "PUT" };
		private static IConfiguration _configuration;

		public ApiApplicationBuilder() { }

		public virtual void StartUp(IConfiguration configuration, IWebHostEnvironment env)
		{
			_configuration = configuration;
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
			try
			{
				/// configure MVC for Api only keeping support for .NewtonsoftJson                                  
				services.AddControllers(option =>
				{
					option.EnableEndpointRouting = false;
					option.SuppressAsyncSuffixInActionNames = false;
				})   // web api mvc support only
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff";
					options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
					options.SerializerSettings.Converters = new List<JsonConverter> { new StringEnumConverter { AllowIntegerValues = true } };
				});

				// Access - Control - Allow - Origin
				services.AddCors(options =>
				{
					options.AddPolicy(_CorsPolicy,
						builder =>
						{
							var allowedHosts = _configuration.GetAppSettingValue(ConfigurationKeyType.AllowedHosts);
							if (string.IsNullOrEmpty(allowedHosts) || allowedHosts.Equals("*"))
							{
								builder.AllowAnyOrigin();
							}
							builder.WithMethods(_allowedMethods).AllowAnyHeader();
						});
				});

				IConnectionConfig config = new DbConnectionConfig(_configuration).DbConnection("");
			    services.AddSingleton<IConnectionConfig>(config) ;

				services.AddSingleton<IConfiguration>(_configuration);

				/// swagger documentation of Api
				ConfigureServicesForApiDocumentation(services, "test api name", "test api version", "test api description");

			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}


		public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			try
			{
				/// Configure api doc with swagger
				ConfigureApiDocumentation(app, "test aApi Name");

				/// error handling setup
				if (env.IsDevelopment())
				{
					app.UseDeveloperExceptionPage();
				}
				else
				{   /////////////////////////////////////
					/// custom error handler controller 
					app.UseExceptionHandler("/error");
				}

				app.UseStaticFiles();
				app.UseCors(_CorsPolicy);

				app.UseRouting();
			//	app.UseAuthentication();
				app.UseAuthorization();

				app.UseEndpoints(endpoints =>
				{
					endpoints.MapControllers();
				});


			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		#region Helper method to get connection string and details

		//public class DbConnectionConfig1 : IConnectionConfig
		//{
		//	public string ConnectionString { get; set; }
		//	public int CommandTimeout { get; set; }
		//	public DbConnectionConfig1()
		//	{
		//		ConnectionString = "Data Source=S103573-W101;Initial Catalog=CPM202GAToCU9_Badgers_Metadata;Integrated Security=True;Application Name=\"Sunrise Clinical Analytics Web Application\"";
		//		CommandTimeout = 30;
		//	}
		//}

		#endregion


		#region Helper method for swag-documentation

		/// <summary>
		/// Configures application for Swagger documentation support using apiName as the endpoint title
		/// </summary>
		/// <param name="app">application builder</param>
		/// <param name="apiName">Name of api endpoint (title)</param>
		private void ConfigureApiDocumentation(IApplicationBuilder app, string apiName)
		{
			if (bool.Parse(_configuration[ConfigurationKeyType.App.SwaggerEnabled] ?? "false"))
			{
				if (string.IsNullOrEmpty(apiName)) throw new ArgumentNullException("Api documentation configuration documentation error - apiName is undefined and is required for api documentation and can be set calling SetApiDocumenationInfo().");
				app.UseSwagger();

				// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
				// specifying the Swagger JSON endpoint.
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("./swagger/v1/swagger.json", "TestApiName" ?? "Api");
					c.RoutePrefix = string.Empty;
				});
			}
		}


		/// <summary>
		/// Configures services for Swagger documentation support using apiName as the endpoint title
		/// </summary>
		/// <param name="services">application services</param>
		/// <param name="apiName">Name of api endpoint (title)</param>
		/// <param name="version">Api version</param>
		/// <param name="description">description of Api application</param>
		private void ConfigureServicesForApiDocumentation(IServiceCollection services, string apiName, string version, string description)
		{
			if (bool.Parse(_configuration[ConfigurationKeyType.App.SwaggerEnabled] ?? "false"))
			{
				if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(apiName) || string.IsNullOrEmpty(description)) throw new ArgumentNullException("Api documentation configuration error - apiName/Version/Description are undefined and is required for api documentation and can be set calling SetApiDocumenationInfo().");
				services.AddSwaggerGen(c =>
				{
					c.SwaggerDoc(version, new OpenApiInfo { Title = apiName, Version = version, Description = description });
				});
			}
		}
        #endregion

    }
}
