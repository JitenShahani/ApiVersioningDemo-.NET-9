namespace ApiVersioningDemo.MinimalEndpoints;

public class MyEndpoints
{
	public void MapMyEndpoints (WebApplication app)
	{
		// Configure Minimal endpoint to support API Versioning
		var versionSet = app.NewApiVersionSet ()
			.HasApiVersion (2, 0)
			.HasDeprecatedApiVersion (1, 0)
			.ReportApiVersions ()
			.Build ();

		var myGroup = app.MapGroup ("/v{version:apiVersion}/hello")
			.WithApiVersionSet (versionSet)
			.WithTags ("Minimal Endpoints")
			.WithSummary ("Hello, World!");

		myGroup.MapGet ("", [Obsolete ("Deprecated")] (HttpContext httpContext) =>
		{
			var apiVersion = $"v{httpContext.GetRequestedApiVersion ()}";

			return TypedResults.Ok ($"Hello, World! - {apiVersion}");
		})
			.HasApiVersion (new ApiVersion (1, 0))
			.Deprecated ()
			.WithOpenApi(operation =>
			{
				operation.Deprecated = true;

				return operation;
			});

		myGroup.MapGet ("", (HttpContext httpContext) =>
		{
			var apiVersion = $"v{httpContext.GetRequestedApiVersion ()}";

			return TypedResults.Ok ($"Hello, World! - {apiVersion}");
		})
			.HasApiVersion (new ApiVersion (2, 0));
	}
}