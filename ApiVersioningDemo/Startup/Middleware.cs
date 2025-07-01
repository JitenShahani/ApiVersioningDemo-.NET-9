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
			// Configure Open Api
			app.MapOpenApi ();

			// Configure Swagger
			app.UseSwagger (c => c.RouteTemplate = "openApi/{documentName}.json");
			app.UseSwaggerUI (options =>
			{
				var descriptions = app.DescribeApiVersions ();

				// Enable the "Try it out" button out of the box.
				options.EnableTryItOutByDefault ();

				// Display the request duration at the end of the response.
				options.DisplayRequestDuration ();

				// Collapse all the tags & schema sections.
				options.DocExpansion (DocExpansion.None);

				// Render example in the Model tab.
				options.DefaultModelRendering (ModelRendering.Example);

				foreach (var description in descriptions.OrderByDescending (d => d.GroupName))
				{
					var url = $"/openApi/{description.GroupName}.json";

					var name = description.IsDeprecated
						? $"{description.GroupName.ToUpperInvariant ()} (DEPRECATED)"
						: $"{description.GroupName.ToUpperInvariant ()}";

					// Add a Swagger endpoint for each API version.
					options.SwaggerEndpoint (url, name);
				}
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
					.WithTitle ("Keyed Services Demo")
					.WithDownloadButton (true)
					.WithDefaultOpenAllTags (false)
					.WithFavicon ("https://scalar.com/logo-light.svg")
					.WithDefaultHttpClient (ScalarTarget.CSharp, ScalarClient.HttpClient);

				var descriptions = app.DescribeApiVersions ();

				foreach (var description in descriptions.OrderByDescending (d => d.GroupName))
				{
					var url = $"/openApi/{description.GroupName}.json";

					var name = description.IsDeprecated
						? $"{description.GroupName.ToUpperInvariant ()} (DEPRECATED)"
						: $"{description.GroupName.ToUpperInvariant ()}";

					options.AddDocument ("Keyed Services Demo", name, url);
				}
			});
		}

		app.UseHttpsRedirection ();

		// Map controller based endpoints
		app.MapControllers ();
	}
}