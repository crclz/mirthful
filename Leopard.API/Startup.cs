using Leopard.API.Filters;
using Leopard.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Leopard.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Secret store for JWT
			// TODO: put secret in env in docker-compose
			var secret = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45,
				170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };
			services.AddSingleton(s => new SecretStore(secret));

			services.AddControllers(p => p.Filters.Add(new MyModelFilter())).AddNewtonsoftJson();

			services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "TheAPI", Version = "v1" });
				c.CustomOperationIds(apiDesc =>
				{
					return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
				});

				// Without this, client cannot send correct data
				c.DescribeAllEnumsAsStrings();
			});

			// The c# world has get used to the behavior of Newtonsoft.Json.
			// There's no reason of switching to System.Text.Json
			services.AddSwaggerGenNewtonsoftSupport();

			services.AddMediatR(Assembly.GetExecutingAssembly());
			services.AddSingleton(new LeopardDatabase());
			services.AddTransient(typeof(Repository<>));
			services.AddScoped<MiddleStore>();
			services.AddScoped<SessionStore>();

			var blobStorePath = Environment.GetEnvironmentVariable("BLOB_STORE");
			blobStorePath = blobStorePath ?? throw new InvalidOperationException("$BLOB_STORE is null");
			services.AddTransient<IBlobBucket>((s) => new FileBucket(new DirectoryInfo(blobStorePath), "/blob"));

			// add filters
			Assembly.GetExecutingAssembly().GetTypes()
				.Where(p => p.GetInterfaces().Contains(typeof(IAsyncActionFilter)))
				.ToList().ForEach(filterType => services.AddTransient(filterType));

			// add pipeline contexts
			Assembly.GetExecutingAssembly().GetTypes()
				.Where(p => p.GetInterfaces().Contains(typeof(IPipelineContext)))
				.ToList().ForEach(contextType => services.AddScoped(contextType));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			//app.UseHttpsRedirection();

			app.UseStaticFiles(); // For the wwwroot folder

			var blobStorePath = Environment.GetEnvironmentVariable("BLOB_STORE");
			blobStorePath = blobStorePath ?? throw new InvalidOperationException("$BLOB_STORE is null");
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(blobStorePath),
				RequestPath = "/blob",
				ServeUnknownFileTypes = true
			});


			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
