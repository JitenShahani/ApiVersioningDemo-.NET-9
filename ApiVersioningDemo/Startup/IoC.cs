namespace ApiVersioningDemo.Startup;

public static class IoC
{
	public static void ConfigureIoCContainer (this WebApplicationBuilder builder)
	{
		// Configure DI Container to blow up early at builder.Build () instead of run time.
		builder.Host.UseDefaultServiceProvider ((context, options) =>
		{
			// Validate that scoped services are not directly or indirectly resolved from singleton services.
			options.ValidateScopes = true;

			// Validate the service provider during builder.Build() to detect any configuration issues.
			// Remember, you will have to add all required services to a service so the instance can be created.
			// ServiceValidator class is a dummy service that ensures that IEmployee services are registered.
			options.ValidateOnBuild = true;
		});

		// Configure Controller support
		builder.Services
			.AddControllers ()
			.AddJsonOptions (options =>
			{
				// Configure JSON serializer to ignore null values during serialization
				options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
				// Configure JSON serializer to use Pascal case for property names during serialization
				options.JsonSerializerOptions.PropertyNamingPolicy = null;
				// Configure JSON serializer to use Pascal case for key's name during serialization
				options.JsonSerializerOptions.DictionaryKeyPolicy = null;
				// Ensure JSON property names are not case-sensitive during deserialization
				options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				// Prevent serialization issues caused by cyclic relationships in EF Core entities
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
				// Ensure the JSON output is consistently formatted for readability.
				// Not to be used in Production as the response message size could be large
				// options.JsonSerializerOptions.WriteIndented = true;
			});

		// Configure Problem Details
		builder.Services.AddProblemDetails (options =>
		{
			options.CustomizeProblemDetails = context =>
			{
				context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

				context.ProblemDetails.Extensions.TryAdd ("requestId", context.HttpContext.TraceIdentifier);
			};
		});

		// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
		builder.Services.AddOpenApi ();

		// Adds support for API endpoint discovery and documentation generation
		builder.Services.AddEndpointsApiExplorer ();

		// Configure OpenAPI version specific documents
		// builder.Services.AddOpenApi ("v1", options =>
		// {
		// 	options.AddDocumentTransformer ((document, context, cancellationToken) =>
		// 	{
		// 		document.Info = new OpenApiInfo
		// 		{
		// 			Version = "v1",
		// 			Title = "API Versioning Demo v1",
		// 			Description = "⚠️ This version is deprecated."
		// 		};

		// 		return Task.CompletedTask;
		// 	});

		// 	options.AddOperationTransformer ((operation, context, cancellationToken) =>
		// 	{
		// 		operation.Deprecated = true;
		// 		return Task.CompletedTask;
		// 	});
		// });

		// builder.Services.AddOpenApi ("v2", options =>
		// {
		// 	options.AddDocumentTransformer ((document, context, cancellationToken) =>
		// 	{
		// 		document.Info = new OpenApiInfo
		// 		{
		// 			Version = "v2",
		// 			Title = "API Versioning Demo v2"
		// 		};

		// 		return Task.CompletedTask;
		// 	});
		// });

		// Configure Swagger for Api Versioning
		// No longer needed. Refer AddSwaggerGen configuration instead.
		// builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions> ();
		// builder.Services.AddSwaggerGen (options => options.OperationFilter<SwaggerDefaultValues> ());

		// Configure Swagger version specific documents
		builder.Services.AddSwaggerGen (options =>
		{
			options.SwaggerDoc ("v1", new OpenApiInfo
			{
				Version = "v1",
				Title = "API Versioning Demo v1",
				Description = "⚠️ This version is deprecated."
			});
			options.SwaggerDoc ("v2", new OpenApiInfo
			{
				Version = "v2",
				Title = "API Versioning Demo v2"
			});
		});

		// Configure API Versioning
		builder.Services
			.AddApiVersioning (options =>
			{
				// Set up the default API Version
				options.DefaultApiVersion = new ApiVersion (2, 0);

				// If version is unspecified, make sure to use the default API version mentioned above
				options.AssumeDefaultVersionWhenUnspecified = true;

				// Report supported API version in response header
				options.ReportApiVersions = true;

				// Use API base route to version endpoints
				options.ApiVersionReader = new UrlSegmentApiVersionReader ();
			}).AddMvc (options =>
			{
				options.Conventions.Add (new VersionByNamespaceConvention ());
			})
			.AddApiExplorer (options =>
			{
				options.GroupNameFormat = "'v'V";
				options.SubstituteApiVersionInUrl = true;
			});

		// Register IEmployee Services
		builder.Services.AddKeyedSingleton<IEmployee, EmployeeService> ("employeeService");
		builder.Services.AddKeyedSingleton<IEmployee, TempEmployeeService> ("tempEmployeeService");

		// This service ensures that IEmployee services are registered. If not, fail-fast on builder.Build().
		builder.Services.AddSingleton<ServiceValidator> ();
	}
}