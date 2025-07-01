namespace ApiVersioningDemo.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;

	public ConfigureSwaggerOptions (IApiVersionDescriptionProvider provider) =>
		_provider = provider;

	public void Configure (SwaggerGenOptions options)
	{
		// add a swagger document for each discovered API version
		// note: you might choose to skip or document deprecated API versions differently
		foreach (var description in _provider
			.ApiVersionDescriptions
			.OrderByDescending(d => d.ApiVersion))
		{
			options.SwaggerDoc (description.GroupName, CreateInfoForApiVersion (description));
		}
	}

	private static OpenApiInfo CreateInfoForApiVersion (ApiVersionDescription description)
	{
		var text = new StringBuilder ("This API contains all the endpoints demonstrating Keyed Services and its injection. An example application with OpenAPI, Swashbuckle, and API versioning.");

		var info = new OpenApiInfo ()
		{
			Title = $"Keyed Services Demo",
			Version = description.ApiVersion.ToString (),
			Contact = new OpenApiContact () { Name = "Jiten Shahani", Email = "shahani.jiten@gmail.com" },
			License = new OpenApiLicense () { Name = "MIT", Url = new Uri ("https://opensource.org/licenses/MIT") }
		};

		if (description.IsDeprecated)
			text.Append ("\n\nThis API version has been deprecated.");

		if (description.SunsetPolicy is { } policy)
		{
			if (policy.Date is { } when)
			{
				text.Append (" The API will be sunset on ")
					.Append (when.Date.ToShortDateString ())
					.Append ('.');
			}

			if (policy.HasLinks)
			{
				text.AppendLine ();

				var rendered = false;

				for (var i = 0; i < policy.Links.Count; i++)
				{
					var link = policy.Links[i];

					if (link.Type == "text/html")
					{
						if (!rendered)
						{
							text.Append ("<h4>Links</h4><ul>");
							rendered = true;
						}

						text.Append ("<li><a href=\"");
						text.Append (link.LinkTarget.OriginalString);
						text.Append ("\">");
						text.Append (
							StringSegment.IsNullOrEmpty (link.Title)
							? link.LinkTarget.OriginalString
							: link.Title.ToString ());
						text.Append ("</a></li>");
					}
				}

				if (rendered)
				{
					text.Append ("</ul>");
				}
			}
		}

		//text.Append ("<h4>Additional Information</h4>");
		info.Description = text.ToString ();

		return info;
	}
}