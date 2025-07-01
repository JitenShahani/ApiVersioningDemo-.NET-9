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
			// ServiceValidator class is a dummy class that ensures that IEmployee services are registered.
			options.ValidateOnBuild = true;
		});

		// Configure Controller support
		builder.Services.AddControllers ().AddJsonOptions (options =>
		{
			// Configure JSON serializer to ignore null values during serialization
			options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			// Configure JSON serializer to use Pascal case for property names during serialization
			options.JsonSerializerOptions.PropertyNamingPolicy = null;
			// Configure JSON serializer to use Pascal case
			options.JsonSerializerOptions.DictionaryKeyPolicy = null;
			// Ensure JSON property names are not case-sensitive during deserialization
			options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
			// Prevent serialization issues caused by cyclic relationships in EF Core entities
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
			// Ensure the JSON output is consistently formatted for readability
			options.JsonSerializerOptions.WriteIndented = true;
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

		// Configure Swagger for Api Versioning
		builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions> ();
		builder.Services.AddSwaggerGen (options => options.OperationFilter<SwaggerDefaultValues> ());

		// Configure API Versioning
		builder.Services.AddApiVersioning (options =>
		{
			options.ReportApiVersions = true;
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.DefaultApiVersion = new ApiVersion (2, 0);
			options.ApiVersionReader = new UrlSegmentApiVersionReader ();

			//options.ApiVersionReader = ApiVersionReader.Combine (
			//	new UrlSegmentApiVersionReader (),
			//	new QueryStringApiVersionReader (), // defaults to api-version
			//	new HeaderApiVersionReader ("X-Api-Version"),
			//	new MediaTypeApiVersionReader ("api-version"));
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
		builder.Services.AddKeyedSingleton<IEmployee, EmployeeRepository> ("employeeRepo");
		builder.Services.AddKeyedSingleton<IEmployee, TempEmployeeRepository> ("tempEmployeeRepo");

		// This service ensures that IEmployee services are registered. If not, above line of code will make sure an exception is thrown.
		builder.Services.AddSingleton<ServiceValidator> ();
	}
}