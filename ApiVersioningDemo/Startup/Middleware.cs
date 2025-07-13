namespace ApiVersioningDemo.Startup;

public static class Middleware
{
	public static void ConfigurePipeline (this WebApplication app)
	{
		// Enable exception handling middleware to handle errors globally
		app.UseExceptionHandler ();

		// Enable status code pages to provide detailed error responses for HTTP status codes
		app.UseStatusCodePages ();

		if (app.Environment.IsDevelopment ())
		{
			var versionDescriptions = app.DescribeApiVersions ();

			var versionList = versionDescriptions
				.OrderByDescending (d => d.GroupName)
				.Select (d => new
				{
					d.GroupName,
					d.ApiVersion,
					Url = $"/openApi/{d.GroupName}.json",
					Name =
						d.IsDeprecated
							? $"{d.GroupName.ToUpperInvariant ()} (DEPRECATED)"
							: $"{d.GroupName.ToUpperInvariant ()}"
				})
				.ToList ();

			Console.WriteLine ("Discovered API versions:");

			foreach (var description in versionList)
				Console.WriteLine ($"- {description.GroupName} (v{description.ApiVersion})");

			// Configure Open Api
			// app.MapOpenApi ("openApi/{documentName}.json");

			// Configure Swagger
			app.UseSwagger (c => c.RouteTemplate = "openApi/{documentName}.json");
			app.UseSwaggerUI (options =>
			{
				// Add Document Title for Swagger UI
				options.DocumentTitle = "API Versioning Demo";

				// Collapse all the tags & schema sections.
				options.DocExpansion (DocExpansion.None);

				// Enable deep linking for tags and operations in the URL.
				options.EnableDeepLinking ();

				// Enable filtering of the operations based on Tags.
				options.EnableFilter ();

				// Enable the validator badge.
				options.EnableValidator ();

				// Enable the "Try it out" button out of the box.
				options.EnableTryItOutByDefault ();

				// Display OperationId for all endpoints.
				options.DisplayOperationId ();

				// Display the request duration at the end of the response.
				options.DisplayRequestDuration ();

				// Render example in the Model tab.
				options.DefaultModelRendering (ModelRendering.Example);

				// Set the default model expand depth to 4.
				options.DefaultModelExpandDepth (4);

				// Set the default model expand depth to 4.
				options.DefaultModelsExpandDepth (4);

				// Define the Swagger endpoints.
				foreach (var description in versionList)
					options.SwaggerEndpoint (description.Url, description.Name);
			});

			// Configure Scalar
			app.MapScalarApiReference (options =>
			{
				options
					.WithSidebar (true)
					.WithTagSorter (TagSorter.Alpha)
					.WithLayout (ScalarLayout.Modern)
					.WithClientButton (false)
					.WithTheme (ScalarTheme.BluePlanet)
					.WithTitle ("API Versioning Demo")
					.WithDocumentDownloadType (DocumentDownloadType.Both)
					.WithDefaultOpenAllTags (false)
					.WithFavicon ("https://scalar.com/logo-light.svg")
					.WithDefaultHttpClient (ScalarTarget.CSharp, ScalarClient.HttpClient);

				var serverPort = app.Urls.FirstOrDefault ()?.Split (':').Last ();

				// Scalar uses the servers section from the OpenAPI document.
				// If no server is explicitly set, it defaults to http://localhost without inspecting the actual port.
				// Swagger, uses middleware that dynamically reads the request context and adjusts accordingly.
				// Added as a caution
				options.AddServer ($"http://localhost:{serverPort}");

				// Define Scalar endpoints.
				foreach (var description in versionList)
					options.AddDocument ("API Versioning Demo", description.Name, description.Url);
			});
		}

		app.UseHttpsRedirection ();

		// Map controller based endpoints
		app.MapControllers ();
	}
}