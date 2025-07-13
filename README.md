<!--
## 📚 Table of Contents

- [🔄 API Versioning Demo](#-api-versioning-demo)
- [🎯 Key Objectives](#-key-objectives)
- [📂 Project Structure](#-project-structure)
- [🌐 Launch Profiles & UI Clients](#-launch-profiles--ui-clients)
- [📌 Endpoints](#-endpoints)
- [🔢 API Versioning Strategies](#-api-versioning-strategies)
	- [1. Query Parameter Versioning](#1-query-parameter-versioning)
	- [2. Header-Based Versioning](#2-header-based-versioning)
	- [3. Media Type Versioning](#3-media-type-versioning)
	- [4. URL Segment Versioning (Used in this Project)](#4-url-segment-versioning-used-in-this-project)
- [⚙️ Configuration Notes](#-configuration-notes)
	- [🧩 IoC Configuration & Registration](#ioc-configuration--registration)
		- [🛡️ Validation Behavior](#validation-behavior)
		- [📘 Controller & Serialization Setup](#controller--serialization-setup)
		- [🩺 Problem Details Configuration](#problem-details-configuration)
		- [📚 OpenAPI & Documentation](#openapi--documentation)
		- [🔑 Keyed DI & Validation](#keyed-di--validation)
	- [🧾 DTOs](#dtos)
		- [Response.cs](#responsecs)
		- [WeatherForecast.cs](#weatherforecastcs)
	- [🛠️ Services](#services)
		- [IEmployee.cs](#iemployeecs)
		- [EmployeeService.cs](#employeeservicecs)
		- [TempEmployeeService.cs](#tempemployeeservicecs)
		- [ServiceValidator.cs](#servicevalidatorcs)
- [🔧 API Versioning Setup](#api-versioning-setup)
	- [🧭 How to Configure API Versioning](#how-to-configure-api-versioning)
	- [🧬 Generate Version-Specific Documents](#generate-version-specific-documents)
		- [🔹 Configure Using OpenApi](#configure-using-openapi)
		- [🔹 Configure Using Swashbuckle](#configure-using-swashbuckle)
- [📘 UI Client Integration Overview](#ui-client-integration-overview)
	- [🛠️ Configure Swagger UI](#configure-swagger-ui)
	- [🛠️ Configure Scalar](#configure-scalar)
- [🔎 Versioned Endpoint Samples](#versioned-endpoint-samples)
	- [📂 Controller-Based Endpoints](#controller-based-endpoints)
	- [📂 Minimal API Endpoints](#minimal-api-endpoints)
- [🧠 Hidden Gotchas & Best Practices](#hidden-gotchas--best-practices)
- [🔬 Advanced Considerations](#-advanced-considerations)
- [📚 References](#-references)
- [🧭 Stay Curious. Build Thoughtfully.](#_stay-curious-build-thoughtfully-)
-->

# 🔄 API Versioning Demo

[![Asp.Versioning.Mvc](https://img.shields.io/nuget/dt/Asp.Versioning.Mvc.svg?label=Asp.Versioning.Mvc&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Asp.Versioning.Mvc/)
[![Asp.Versioning.Mvc.ApiExplorer](https://img.shields.io/nuget/dt/Asp.Versioning.Mvc.ApiExplorer.svg?label=Asp.Versioning.Mvc.ApiExplorer&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Asp.Versioning.Mvc.ApiExplorer/)
[![Microsoft.AspNetCore.OpenApi](https://img.shields.io/nuget/dt/Microsoft.AspNetCore.OpenApi.svg?label=Microsoft.AspNetCore.OpenApi&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/)
[![Swashbuckle.AspNetCore](https://img.shields.io/nuget/dt/Swashbuckle.AspNetCore.svg?label=Swashbuckle.AspNetCore&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Swashbuckle.AspNetCore/)
[![Scalar.AspNetCore](https://img.shields.io/nuget/dt/Scalar.AspNetCore.svg?label=Scalar.AspNetCore&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Scalar.AspNetCore/)

This repository demonstrates how to implement **API Versioning** and **Keyed Singleton Services** in an ASP.NET Core Web API using **.NET 9**. It features both **Minimal APIs** and **Controller-based endpoints**, showing how to structure versioned routing, resolve services via constructor injection, and configure flexible, version-aware API documentation using **OpenAPI** and **Swashbuckle**, visualized through both **Swagger UI** and **Scalar**.

## 🎯 Key Objectives

- Implement API versioning using `Asp.Versioning.Mvc`, with `[ApiVersion]` and `.HasApiVersion (...)` and URL segment routing across controllers and minimal endpoints.
- Register and resolve multiple `IEmployee` implementations as keyed singletons using `[FromKeyedServices (...)]`, injected exclusively via constructor parameters.
- Enforce fail-fast validation at startup using `ServiceValidator`, ensuring required services are registered during `builder.Build()`.
- Integrate versioned OpenAPI documentation using the official OpenAPI Specification, surfaced through `Swagger UI` and `Scalar` interfaces with grouped endpoints and deprecation metadata.
- Maintain a predictable structure by separating endpoint behavior into minimal definitions and versioned controllers.

## 📂 Project Structure

```
ApiVersioningDemo/
├── Controllers/
│   ├── v1/
│   │   └── KeyedServiceController.cs	v1 controller (deprecated)
│   ├── v2/
│   │   └── KeyedServiceController.cs	v2 controller
│   └── WeatherForecastController.cs	Version-neutral controller
├── Dto/
│   ├── Response.cs			DTO for API responses
│   └── WeatherForecast.cs		DTO for Weather forecast
├── MinimalEndpoints/
│   ├── MyEndpoints.cs			Minimal Endpoints with both v1 & v2
├── Services/
│   ├── IEmployee.cs			Employee Service interface
│   ├── EmployeeService.cs		Main Employee service
│   ├── TempEmployeeService.cs		Temporary Employee service
│   └── ServiceValidator.cs		Validates that all required keyed IEmployee services are registered at startup.
│					This "dummy" service is injected with both `employeeRepo` and `tempEmployeeRepo` keyed services.
│					This ensures the DI container throws an error during app build if any are missing or misconfigured.
│					This provides early, fail-fast validation of your dependency injection setup.
├── Startup/
│   ├── IoC.cs				Dependency injection and service registration
│   └── Middleware.cs			Middleware and pipeline config
└── Swagger/				These classes are no longer used in this project but are included for future reference.
    ├── ConfigureSwaggerOptions.cs	Swashbuckle/OpenAPI configuration
    └── SwaggerDefaultValues.cs		Swashbuckle/OpenAPI operation filter
```

## 🌐 Launch Profiles & UI Clients
The current launch settings is configured to dynamically load one of the following UIs:

| Launch Profile           | API Explorer Loaded |
|--------------------------|---------------------|
| HTTP                     | Swagger UI			 |
| HTTPS / IIS Express      | Scalar				 |

> 💡 Tip: You can change the launch profile by editing `Properties/launchSettings.json`. Alternatively, you can launch Swagger UI by visiting `/swagger` & Scalar by visiting `/scalar`.

## 📌 Endpoints

| Version			| Route									| Description								|
|-------------------|---------------------------------------|-------------------------------------------|
| v1 (Deprecated)	| GET /api/v1/KeyedService/employee		| Returns Employee message (v1)				|
| v1 (Deprecated)	| GET /api/v1/KeyedService/tempEmployee	| Returns Temp Employee message (v1)		|
| v1 (Deprecated)	| GET /api/v1/KeyedService/both			| Returns both  messages (v1)				|
| v2				| GET /api/v2/KeyedService/employee		| Returns Employee message (v2)				|
| v2				| GET /api/v2/KeyedService/tempEmployee	| Returns Temp Employee message (v2)		|
| v2				| GET /api/v2/KeyedService/both			| Returns both messages (v2)				|
| Version neutral   | GET /WeatherForecast                  | Returns next 5 days weather forecast      |

## 🔢 API Versioning Strategies

This project adopts **URL Segment versioning** for simplicity, visibility, and clean routing. However, ASP.NET Core supports multiple strategies via `IApiVersionReader`, all configurable in `Startup/IoC.cs` under `.AddApiVersioning()`.

ℹ️ Note:

- Only **URL Segment versioning** injects the version into your route path (e.g., `/api/v1/KeyedService/Employee`).
- For other strategies like **Query**, **Header**, or **Media Type**, use `[Route ("api/[controller]")]` and let the versioning system resolve the correct endpoint via metadata.

Here’s a quick reference to the four supported approaches:

### 1. ❓ Query Parameter Versioning

```csharp
builder.Services.AddApiVersioning (options =>
{
	...
	options.ApiVersionReader = new QueryStringApiVersionReader();
});
```

```http
GET http://localhost:xxxx/api/KeyedService/employee?api-version=1.0
```

- The default query string key is `api-version`, but it can be changed (e.g., new QueryStringApiVersionReader ("version")).
- Easy to test manually via browser or Postman.
- Less cache-friendly and can clutter URLs with version metadata.

### 2. 📬 Header-Based Versioning

```csharp
builder.Services.AddApiVersioning (options =>
{
	...
	options.ApiVersionReader = new HeaderApiVersionReader ("X-Api-Version");
});
```

```http
GET http://localhost:xxxx/api/KeyedService/employee
Header: X-Api-Version: 1.0
```

- Default header name is `api-version`, but can be customized as shown above.
- Keeps URLs clean and version metadata out of the path.
- Client tooling must explicitly include the version header. Not browser-friendly by default.

### 3. 📰 Media Type Versioning

```csharp
builder.Services.AddApiVersioning (options =>
{
	...
	options.ApiVersionReader = new MediaTypeApiVersionReader();
});
```

```http
GET http://localhost:xxxx/api/KeyedService/employee
Accept: application/json; v=1.0
```

- Default version key is `v`.
- Supports content negotiation and advanced versioning scenarios.
- Typically used in APIs that serve multiple formats or negotiated schemas.
- Requires consumers to send custom media types. Not intuitive for manual testing.

### 4. 🌐 URL Segment Versioning (Used in this Project)

```csharp
builder.Services.AddApiVersioning (options =>
{
	...
	options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
```

```http
GET http://localhost:xxxx/api/v1/KeyedService/employee
```

- Injects version directly into the request path (/v1/, /v2/, etc).
- Requires explicit route configuration in both Controllers and Minimal APIs.
	- Controllers: `[Route ("api/{version:apiVersion}/[controller]")]`
	- Minimal APIs: `/api/v{version:apiVersion}/...` route paths with `.HasApiVersion(...)` and `.WithApiVersionSet(...)`
- Enhances discoverability and integrates seamlessly with Controllers and Minimal endpoints. Supports version-specific DI and OpenAPI grouping for tools like Swagger UI and Scalar.

## ⚙️ Configuration Notes

This section provides implementation-focused insight into how each layer of the app is wired, from DI registration to Middleware, DTOs, Controllers, and Minimal endpoints. Each block references actual source code and highlights its responsibilities.

### 🧩 IoC Configuration & Registration

Covers Scoped validation, JSON Serialization, API Versioning, and Service registration.

#### 🛡️ Validation Behavior

```csharp
builder.Host.UseDefaultServiceProvider ((context, options) =>
{
	options.ValidateScopes = true;
	options.ValidateOnBuild = true;
});
```

Enables proactive error detection during service registration:

- `ValidateScopes` ensures that scoped services are not mistakenly consumed by singletons. A common DI pitfall.
- `ValidateOnBuild` performs early validation of the service graph during `builder.Build()` instead of waiting for runtime resolution.
- Combined with `ServiceValidator`, this pattern enforces fail-fast container setup, helping developers catch misconfigurations immediately when the app starts.

#### 📘 Controller & Serialization Setup

```csharp
builder.Services
	.AddControllers ()
	.AddJsonOptions (options =>
	{
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		options.JsonSerializerOptions.PropertyNamingPolicy = null;
		options.JsonSerializerOptions.DictionaryKeyPolicy = null;
		options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});
```

> Configures JSON serialization to output Pascal-case keys, ignore null values, and avoid circular references especially useful for DTO consistency and EF Core entities.

#### 🩺 Problem Details Configuration

```csharp
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});
```

> Adds diagnostic metadata to error responses (e.g., HTTP method, path, request ID) for easier traceability in development and debugging workflows.

#### 📚 OpenAPI & Documentation

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
```

> These registrations prepare the application to expose OpenAPI-compatible metadata and endpoint descriptions. `AddOpenApi()` adds baseline support for spec generation and is typically used in conjunction with tools like Swagger UI and Scalar.

#### 🔑 Keyed DI & Validation

```csharp
builder.Services.AddKeyedSingleton<IEmployee, EmployeeService>("employeeService");
builder.Services.AddKeyedSingleton<IEmployee, TempEmployeeService>("tempEmployeeService");
builder.Services.AddSingleton<ServiceValidator>();
```

Registers two implementations of `IEmployee` using keyed dependency injection, allowing versioned Controllers or Minimal APIs to request specific services via `[FromKeyedServices("...")]`. This pattern avoids conditional resolution logic and supports clean separation of behavior across versions. The `ServiceValidator` class is a dummy consumer of both keyed services injected at startup to trigger container validation. Combined with `.ValidateOnBuild`, it ensures all required dependencies are registered before runtime.

### 🧾 DTOs

Data Transfer Objects define the structure of responses returned by the API.

#### Response.cs

DTO for the versioned controllers `KeyedServiceControllers`.

```csharp
public class Response
{
	public required string Message { get; set; }
}
```

#### WeatherForecast.cs

DTO for the version-neutral `WeatherForecastController`.

```csharp
public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

These DTOs benefit from the `.AddJsonOptions()` configuration applied in `IoC.cs`, which enables:

- Pascal-case formatting
- Filtering of null values
- Case-insensitive property matching
- Cycle-handling for nested relationships

### 🛠️ Services

Contains the logic and contracts used across versioned endpoints and validation layers.

#### 📄 IEmployee.cs

Defines a minimal interface for Employee services, exposing a single method that returns a message or payload.

```csharp
public interface IEmployee
{
	public string GetMessage ();
}
```

#### 👤 EmployeeService.cs

Implements the `IEmployee` interface and returns a static message. Registered with key `employeeService` in DI container.

```csharp
public class EmployeeService : IEmployee
{
	public string GetMessage () => "Hello from Employee Service";
}
```

#### ⛑️ TempEmployeeService.cs

```csharp
public class TempEmployeeService : IEmployee
{
	public string GetMessage () => "Hello from Temporary Employee Service";
}
```

Also implements `IEmployee`, but returns a different message. Registered with the key `tempEmployeeService` in DI container.

#### ✅ ServiceValidator.cs

```csharp
public class ServiceValidator
{
	private readonly IEmployee _employeeService;
	private readonly IEmployee _tempEmployeeService;

	public ServiceValidator (
		[FromKeyedServices ("employeeService")] IEmployee employeeService,
		[FromKeyedServices ("tempEmployeeService")] IEmployee tempEmployeeService) =>
			(_employeeService, _tempEmployeeService) = (employeeService, tempEmployeeService);
}
```

> A DI validation utility that ensures both `employeeService` and `tempEmployeeService` are registered. It’s injected during application startup and works hand-in-hand with `.ValidateOnBuild = true`, enforcing fail-fast service validation. This class has no runtime logic. Its sole purpose is container integrity.

### 🔧 API Versioning Setup

This section demonstrates how to configure API Versioning and integrate it with Swagger UI and Scalar interfaces for version-aware documentation and routing.

#### 🧭 How to Configure API Versioning

API Versioning in this project is powered by `Asp.Versioning.Mvc` and its companion `Asp.Versioning.Mvc.ApiExplorer`. These packages provide endpoint version routing, controller tagging via `[ApiVersion(...)]`, and OpenAPI grouping support for Swagger UI and Scalar documentation.

Here’s how it's configured in `Startup/IoC.cs`:

```csharp
builder.Services
	.AddApiVersioning(options =>
	{
		options.DefaultApiVersion = new ApiVersion(2, 0);
		options.AssumeDefaultVersionWhenUnspecified = true;
		options.ReportApiVersions = true;
		options.ApiVersionReader = new UrlSegmentApiVersionReader();
	})
	.AddMvc(options =>
	{
		options.Conventions.Add(new VersionByNamespaceConvention());
	})
	.AddApiExplorer(options =>
	{
		options.GroupNameFormat = "'v'V";
		options.SubstituteApiVersionInUrl = true;
	});
```

🧠 Key Concepts

| Feature								| Purpose																															|
|---------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------|
| `DefaultApiVersion`					| Declares v2.0 as the supported default version.																					|
| `AssumeDefaultVersionWhenUnspecified`	| Enables implicit fallback behavior for unversioned requests, routing them to the default.											|
| `ReportApiVersions`					| Adds version headers like `api-supported-versions` and `api-deprecated-versions` to API responses.								|
| `UrlSegmentApiVersionReader`			| Parses the version from URL segments (e.g., `/api/v2/...`).																		|
| `AddMvc`								| Registers API versioning conventions for MVC-style controllers.																	|
| `VersionByNamespaceConvention`		| Tags controllers with `[ApiVersion]` based on their folder namespace.																|
| `AddApiExplorer`						| Enables endpoint discovery, OpenAPI grouping, and metadata tagging for versioned APIs.											|
| `GroupNameFormat`						| Specifies the format used for version group names in OpenAPI (e.g., `'v'V` renders `v2` for version `2.0`).						|
| `SubstituteApiVersionInUrl`			| Replaces `{version}` tokens in route templates with the actual version number (e.g., `v2`), ensuring correct endpoint routing.	|

Here's how to configure Controllers and Minimal endpoints to implement API Versioning.

🎯 Controller Example

```csharp
[Route("api/{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[ApiExplorerSettings(GroupName = "v2")]
public class KeyedServiceController : ControllerBase
{
    ...
}
```

Controllers must declare both `[ApiVersion(...)]` and `GroupName` for correct documentation grouping.

🔹 Minimal API Example

Minimal APIs declare their version metadata using chained extension methods:

```csharp
app.MapGet("/api/v{version:apiVersion}/hello", () => "Hello, World!")
    .WithOpenApi()
    .WithApiVersionSet (...)
    .HasApiVersion(2.0);
```

This configuration ensures that all endpoints whether Controllers or Minimal APIs are version-aware, grouped correctly in your documentation interfaces, and gracefully handle fallback scenarios. The result is a clean, discoverable, and scalable API structure.

#### 🧬 Generate Version-Specific Documents

The configuration under `Configure API Versioning` is sufficient for internal routing, version metadata tagging, and endpoint grouping. However, if you intend to visualize APIs through UI clients like `Swagger UI` or `Scalar`, you'll need to configure version-specific OpenAPI documents.

ASP.NET Core supports two options for generating versioned documentation:

| Approach			| Trade-offs																								|
|-------------------|-----------------------------------------------------------------------------------------------------------|
| `AddOpenApi()`	| Deprecated endpoints do not show correct metadata unless explicitly configured via transformers.		|
| `AddSwaggerGen()`	| Since Swagger fully owns the document generation process, all configuration even if already set up in `AddOpenApi()` must be **manually redefined**, including authentication, filters, and schema options. |

Choose the approach that best fits your documentation goals:

- If you want native integration with minimal setup, `AddOpenApi()` is ideal.
- If you need full control over the documentation output, `AddSwaggerGen()` gives you that but requires duplication of effort.

> 🔧 Choose one based on whether you want to reuse the .NET-native OpenAPI infrastructure (`AddOpenApi`) or prefer Swashbuckle's customization power (`AddSwaggerGen`).

##### 🔹 Configure Using OpenApi

```csharp
// IoC.cs
builder.Services.AddOpenApi ("v1", options =>
{
	options.AddDocumentTransformer ((document, context, cancellationToken) =>
	{
		document.Info = new OpenApiInfo
		{
			Version = "v1",
			Title = "API Versioning Demo v1",
			Description = "⚠️ This version is deprecated."
		};

		return Task.CompletedTask;
	});

	options.AddOperationTransformer ((operation, context, cancellationToken) =>
	{
		operation.Deprecated = true;
		return Task.CompletedTask;
	});
});

builder.Services.AddOpenApi ("v2", options =>
{
	options.AddDocumentTransformer ((document, context, cancellationToken) =>
	{
		document.Info = new OpenApiInfo
		{
			Version = "v2",
			Title = "API Versioning Demo v2"
		};

		return Task.CompletedTask;
	});
});

// Middleware.cs
app.MapOpenApi ("openApi/{documentName}.json");
```

✅ This setup registers versioned OpenAPI documents using `AddOpenApi(...)`, configured to serve them at `/openApi/v1.json`, `/openApi/v2.json`, etc. Paths are standardized via middleware configuration. These documents are automatically grouped by version and compatible with UI clients like `Swagger UI` and `Scalar`.

> ℹ️ Note: While the `AddOpenApi()` configuration successfully marks version 1 as deprecated, it applies this metadata globally including to version-neutral endpoints. To avoid unintended deprecation flags across the board, this implementation is currently commented out in favor of a more granular setup using Swashbuckle's `AddSwaggerGen()`.

##### 🔹 Configure Using Swashbuckle

```csharp
// IoC.cs
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Versioning Demo v1",
        Description = "⚠️ This version is deprecated."
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "API Versioning Demo v2"
    });
});

// Middleware.cs
app.UseSwagger (c => c.RouteTemplate = "openApi/{documentName}.json");
```

✅ This configuration uses Swashbuckle's `AddSwaggerGen(...)` to generate versioned OpenAPI documents. These are served at `/openApi/v1.json`, `/openApi/v2.json`, matching the same route template. Any existing metadata or settings from AddOpenApi() do not apply. Swashbuckle owns the generation process and must be re-configured independently.

> ℹ️ Note: Use `app.MapOpenApi(...)` in the middleware only when generating documents via `AddOpenApi()`. Use `app.UseSwagger(...)` in the middleware exclusively with `AddSwaggerGen()`. Each middleware binds to its corresponding document generation strategy mixing them can lead to missing endpoints.

> 💡 Note on Deprecated Metadata: For Minimal APIs, applying `[Obsolete("...")]` or using `.Deprecated()` extension method does **not** automatically reflect deprecation in OpenAPI documents. This applies **regardless of whether the documents are generated via `AddOpenApi()` or Swashbuckle's `AddSwaggerGen()`**. To correctly mark a Minimal endpoint as deprecated in your documentation, you must explicitly set it using `.WithOpenApi(operation => { operation.Deprecated = true; return operation; })`.

#### 📘 UI Client Integration Overview

Once versioned OpenAPI documents are registered, they can be rendered using UI clients like `Swagger UI` or `Scalar`. These integrations allow developers to explore grouped endpoints, inspect metadata, and test versioned APIs interactively.

The following sections walks through configuration for each UI client.

##### 🛠️ Configure Swagger UI

Swagger UI enables interactive exploration of your versioned API specs. Once OpenAPI documents are served at known paths, you can configure the UI for optimal readability and developer experience.

```csharp
// Middleware.cs
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
```

🧩 This configuration customizes how Swagger UI behaves when rendering your OpenAPI documents. The `SwaggerEndpoint(...)` bindings dynamically populate tabs for each API version, maintaining grouped discoverability.

##### 🛠️ Configure Scalar

Scalar provides a streamlined, modern interface for exploring versioned OpenAPI documents. Once OpenAPI documents are served at known paths, they can be rendered using Scalar with minimal configuration.

```csharp
// Middleware.cs
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

app.MapScalarApiReference (options =>
{
	options
		.WithSidebar (true)
		.WithTagSorter (TagSorter.Alpha)
		.WithLayout (ScalarLayout.Modern)
		.WithClientButton (false)
		.WithTheme (ScalarTheme.BluePlanet)
		.WithTitle ("Keyed Services Demo")
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
```

> Ensure the favicon URL remains accessible. Replace with a local asset if needed.

🧩 Scalar renders versioned documentation automatically, using the keys registered in the pipeline. It groups endpoints per version and displays embedded metadata such as titles, descriptions, and deprecation tags. No visual customization is required. The UI adapts to document content and reflects it cleanly. Scalar’s minimalist layout offers immediate access to endpoints, tags, request details, and response models, ideal for clean client-side exploration.

#### 🔎 Versioned Endpoint Samples

Here’s how versioned endpoints are declared using both `Controllers` and `Minimal APIs`, showing how `[ApiVersion]`, `GroupName`, and route templates coordinate to ensure version-aware behavior across routing and documentation.

##### 📂 Controller-Based Endpoints

```csharp
// KeyedServiceController.V1

[ApiController]
[Route ("api/v{version:apiVersion}/[controller]")]
[ApiVersion ("1.0", Deprecated = true)]
[ApiExplorerSettings (GroupName = "v1")]
[Obsolete ("Deprecated")]
[Tags ("Controller Endpoints")]
public class KeyedServiceController : ControllerBase
{
	private readonly IEmployee _employeeService;
	private readonly IEmployee _tempEmployeeService;

	public KeyedServiceController (
			[FromKeyedServices ("employeeService")] IEmployee employeeService,
			[FromKeyedServices ("tempEmployeeService")] IEmployee tempEmployeeService) =>
		(_employeeService, _tempEmployeeService) = (employeeService, tempEmployeeService);

	[HttpGet ("employee")]
	[ProducesResponseType<Response> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Employee Message")]
	[EndpointDescription ("This endpoint gets the message from the Employee keyed service.")]
	public IActionResult GetEmployee ()
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		var result = new Response
		{
			Message = $"{_employeeService.GetMessage ()} - {version}"
		};

		return Ok (result);
	}

	[HttpGet ("tempEmployee")]
	[ProducesResponseType<Response[]> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Temporary Employee Message")]
	[EndpointDescription ("This endpoint gets the message from the Temporary Employee keyed service.")]
	public IActionResult GetTempEmployee ()
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		var result = new Response
		{
			Message = $"{_tempEmployeeService.GetMessage ()} - {version}"
		};

		return Ok (result);
	}

	[HttpGet ("both")]
	[ProducesResponseType<Response[]> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Message from Both")]
	[EndpointDescription ("This endpoint gets the message from both Employee & Temporary Employee keyed services.")]
	public IActionResult GetBoth ()
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		Response empMessage = new () { Message = $"{_employeeService.GetMessage ()} - {version}" };
		Response tempEmpMessage = new () { Message = $"{_tempEmployeeService.GetMessage ()} - {version}" };

		Response[] responses = [empMessage, tempEmpMessage];

		return Ok (responses);
	}
}

// KeyedServiceController.V2
[ApiController]
[Route ("api/v{version:apiVersion}/[controller]")]
[ApiVersion ("2.0")]
[ApiExplorerSettings (GroupName = "v2")]
[Tags ("Controller Endpoints")]
public class KeyedServiceController : ControllerBase
{
	private readonly IEmployee _employeeService;
	private readonly IEmployee _tempEmployeeService;

	public KeyedServiceController (
		[FromKeyedServices ("employeeService")] IEmployee employeeService,
		[FromKeyedServices ("tempEmployeeService")] IEmployee tempEmployeeService) =>
			(_employeeService, _tempEmployeeService) = (employeeService, tempEmployeeService);

	[HttpGet ("employee")]
	[ProducesResponseType<Response> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Employee Message")]
	[EndpointDescription ("This endpoint gets the message from the Employee keyed service.")]
	public IActionResult GetEmployee ()
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		var result = new Response
		{
			Message = $"{_employeeService.GetMessage ()} - {version}"
		};

		return Ok (result);
	}

	[HttpGet ("tempEmployee")]
	[ProducesResponseType<Response> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Temporary Employee Message")]
	[EndpointDescription ("This endpoint gets the message from the Temporary Employee keyed service.")]
	public IActionResult GetTempEmployee ()
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		var result = new Response
		{
			Message = $"{_tempEmployeeService.GetMessage ()} - {version}"
		};

		return Ok (result);
	}

	[HttpGet ("both")]
	[ProducesResponseType<Response[]> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Messages from Both")]
	[EndpointDescription ("This endpoint gets the message from both Employee & Temporary Employee keyed services.")]
	public IActionResult GetBoth ()
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		Response empMessage = new () { Message = $"{_employeeService.GetMessage ()} - {version}" };
		Response tempMessage = new () { Message = $"{_tempEmployeeService.GetMessage ()} - {version}" };

		Response[] responses = [empMessage, tempMessage];

		return Ok (responses);
	}
}
```

✅ Controllers define both `[ApiVersion(...)]` and `GroupName` to register their endpoints in version-specific documentation. Routing relies on a URL segment pattern: `api/{version:apiVersion}/...`. Each action returns a typed DTO and uses FromKeyedServices for clean DI resolution.

##### 📂 Minimal API Endpoints

```csharp
// MyEndpoints.cs
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
			.WithOpenApi ()
			.WithApiVersionSet (versionSet)
			.WithTags ("Minimal Endpoints")
			.WithSummary ("Hello, World!");

		myGroup.MapGet ("", [Obsolete ("Deprecated")] (HttpContext httpContext) =>
		{
			var apiVersion = $"v{httpContext.GetRequestedApiVersion ()}";

			return TypedResults.Ok ($"Hello, World! - {apiVersion}");
		})
			.HasApiVersion (new ApiVersion (1, 0))
			.Deprecated ();

		myGroup.MapGet ("", (HttpContext httpContext) =>
		{
			var apiVersion = $"v{httpContext.GetRequestedApiVersion ()}";

			return TypedResults.Ok ($"Hello, World! - {apiVersion}");
		})
			.HasApiVersion (new ApiVersion (2, 0));
	}
}
```

✅ Minimal APIs rely on `ApiVersionSet` for declarative versioning. Use `.NewApiVersionSet()` to define supported versions, then associate it with either endpoint group or endpoints via `.WithApiVersionSet(...)`. Extension methods such as `.HasApiVersion(...)` and `.WithOpenApi()` ensures endpoints are correctly grouped and discoverable in versioned OpenAPI documents. Provided the routing and grouping are consistent, Minimal APIs integrate seamlessly with OpenAPI tooling and client generators.

## 🧠 Hidden Gotchas & Best Practices

Here are a few nuanced tips to keep your versioned API setup clean, maintainable, and discoverable:

- 🧩 Keep `[ApiVersion]` consistent with declared JSON docs. Ensure that your registered OpenAPI documents match the version values used in Controllers and Minimal endpoints. Mismatches lead to invisible routes.
- ⚠️ Don’t forget `ApiExplorerSettings(GroupName = "vX")`. Omitting this leads to missing endpoints in your OpenAPI documents and any downstream tooling that consumes grouped metadata. Grouping is explicit.
- ⚠️ Avoid duplicate route definitions across versions. If two controllers define the same action under the same route but different versions, only one may be rendered if grouping isn’t handled correctly.
- 🛠️ Minimal APIs must use `WithApiVersionSet(...)` for proper registration. Without it, endpoints won’t be grouped correctly in OpenAPI documents, even if they declare a version.

## 🔬 Advanced Considerations

The following strategies explore subtle behaviors in versioned APIs, middleware sequencing, and documentation accuracy. These patterns are **not included in this demo project**, but they may help deepen your understanding and enhance production setups.

### 🧪 Custom Middleware must respect API Versioning

In advanced scenarios, middleware logic may vary based on the API version being requested such as version-specific rate limiting, logging, or conditional processing. However, it's important to understand how and when versioning is resolved in ASP.NET Core. API versioning in ASP.NET Core is resolved via **routing and model binding**, not through a middleware component. The version is only available in the `HttpContext` **after routing has executed**. If you write middleware that inspects the API version, it must run **after `app.UseRouting()`** but **before endpoint handling** otherwise, the version information will not be available.

Example (Version-Specific Logging Middleware):

```csharp
var app = builder.Build ();

app.UseRouting (); // 🔧 Routing must execute first

app.Use (async (context, next) =>
{
	var version = context.GetRequestedApiVersion (); // Only available after routing

	if (version?.MajorVersion == 2)
	{
		logger.LogInformation("Request received for v2 endpoint");
		// You can apply version-specific logic here
	}

	await next ();
});

app.MapControllers (); // or app.MapGroup (...) for Minimal APIs
```

### 🔀 Endpoint Version discovery in Unit Tests

When testing APIs that use versioning, it's critical to confirm that all versions are registered and discoverable. Silent misconfigurations like a missing `[ApiVersion]` attribute or missing `[[ApiExplorerSettings (GroupName = "vX")]]` attribute or an unregistered OpenAPI document can go unnoticed without programmatic inspection. Use `IApiVersionDescriptionProvider` to auto-discover available versions and assert their metadata.

Example Test Snippet:

```csharp
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ApiVersioningDocumentationTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly WebApplicationFactory<Program> _factory;
	private readonly IApiVersionDescriptionProvider _provider;

	public ApiVersioningDocumentationTests (WebApplicationFactory<Program> factory)
	{
		_factory = factory;
		_provider = factory.Services.GetRequiredService<IApiVersionDescriptionProvider> ();
	}

	[Fact]
	public void AllApiVersions_ShouldBeDiscoverable ()
	{
		Assert.NotEmpty (_provider.ApiVersionDescriptions);

		foreach (var version in _provider.ApiVersionDescriptions)
		{
			Assert.False (string.IsNullOrWhiteSpace (version.GroupName));
			Assert.True (version.ApiVersion.IsDefined ());
		}
	}

	[Fact]
	public void DeprecatedVersions_ShouldBeFlagged ()
	{
		var deprecated = _provider.ApiVersionDescriptions.Where (d => d.IsDeprecated).ToList ();

		Assert.Contains (deprecated, d => d.GroupName == "v1");
	}

	[Theory]
	[InlineData ("v1")]
	[InlineData ("v2")]
	public async Task OpenApiDocument_ShouldBeAvailable_ForEachVersion (string version)
	{
		var client = _factory.CreateClient ();
		var response = await client.GetAsync ($"/openApi/{version}.json");

		Assert.Equal (HttpStatusCode.OK, response.StatusCode);

		var content = await response.Content.ReadAsStringAsync ();
		Assert.Contains ($"\"version\":\"{version}\"", content);
	}

	[Fact]
	public void MinimalAndControllerEndpoints_ShouldBeGroupedByVersion_V1 ()
	{
		var versions = _provider.ApiVersionDescriptions.Select (d => d.GroupName).ToList ();

		// Expect both v1 and v2 groups.
		Assert.Contains ("v1", versions);
		Assert.Contains ("v2", versions);
	}

	// The actual comparison is manual. Alternatively, you can check that the discovered Version Groups are not empty...

	[Fact]
	public void MinimalAndControllerEndpoints_ShouldBeGroupedByVersion_V2 ()
	{
		var versions = _provider.ApiVersionDescriptions.Select (d => d.GroupName).ToList ();

		Assert.All (versions, groupName => Assert.False (string.IsNullOrWhiteSpace(groupName)));
	}

	// Alternatively, the test will fail if a new Version Group are discovered...

	[Fact]
	public void MinimalAndControllerEndpoints_ShouldBeGroupedByVersion_V3 ()
	{
		string[] expectedVersions = [ "v1", "v2" ]; // Update this when you add a new version

		var actualVersions = _provider.ApiVersionDescriptions.Select(d => d.GroupName).ToArray();

		Assert.Equal(expectedVersions, actualVersions);
	}
}
```

> 💡 Tip: You can also use this provider to dynamically seed test cases for example, instead of hardcoding tests for `v1` or `v2`, loop over all discovered versions to generate test data on-the-fly. This approach helps prevent version drift in your test suite. Additionally, the provider can be used to locate version-specific OpenAPI documents, which can then be consumed by `Swagger UI` or `Scalar` tooling to scaffold client SDKs. It also aids in validating rollout strategies such as ensuring deprecated versions are flagged or confirming new versions are correctly registered and exposed.

> 💡 Tip: Ensure these tests run after the service provider is fully built, such as within an integration test using a custom host or a framework like `WebApplicationFactory`. Attempting to resolve this provider during registration will result in a runtime error, as version metadata isn’t available until the full DI container is constructed.

### 📄 Keep XML Comments Version-Aware with IncludeXmlComments()

When using `IncludeXmlComments()` to enrich your OpenAPI documentation via OpenAPI or Swashbuckle, it's important to ensure your XML comment file reflects behavior changes introduced across different API versions. ASP.NET Core supports generating XML documentation by enabling `<GenerateDocumentationFile>true</GenerateDocumentationFile>` in the project file. Swashbuckle can then consume this output to populate metadata fields like summaries, descriptions, and remarks in OpenAPI documents.

#### ⚠️ Gotchas to watch for

- If you modify controller actions or models between versions, outdated XML comments may still appear in the generated documentation unless explicitly updated.
- Applying `<inheritdoc/>` across versions can lead to misleading summaries when behavior diverges in later releases.
- Consumers may rely on OpenAPI metadata to understand endpoint behavior so discrepancies in XML comments can cause confusion and misuse.

#### ✅ Best Practices

- Annotate version-specific methods with custom summaries that reflect their versioned behavior. Avoid reusing inherited comments blindly unless the method's intent truly hasn’t changed.
- Use `<inheritdoc/>` only when the behavior remains consistent across versions otherwise, prefer fresh comments to prevent accidental mismatches.
- If using multiple XML documentation files per version group, organize them clearly and apply them selectively to each OpenAPI document generated by Swashbuckle.
- Add version context in remarks or description tags to help consumers identify versioned behavior:

```xml
///	<summary>
///		Gets a user by ID.
///	</summary>
///	<remarks>
///		This version applies to v2 and introduces role-based filtering.
///	</remarks>
```

### 🔄 Endpoint re-registration during version migration

When migrating from attribute-based routing (typically via Controllers) to Minimal APIs, it's important to review how endpoints are registered across versions. If endpoint mappings are reused without clear separation, they may **collide, override, or shadow one another silently**, especially when both paradigms co-exist temporarily.

#### ⚠️ Gotchas to watch for

- If both Controller and Minimal API endpoints are mapped to the same route (e.g., `/users`), one may override the other depending on registration order and grouping.
- Using identical route templates across versions without grouping (e.g., `MapGet("/users")` in both `v1` and `v2`) can result in unexpected routing or missing documentation.
- Minimal APIs **do not inherit** version metadata automatically. To ensure correct OpenAPI document generation and routing behavior, use `.WithApiVersionSet(...)` either on the route group or on the endpoint and `.HasApiVersion(...)` on individual endpoints inside that group.

#### ✅ Best Practices

- Use `MapGroup("/vX")` to segment Minimal API endpoints by version, and pair it with `WithApiVersionSet(...)` to ensure proper grouping.
- If retiring legacy controller routes, deregister or isolate them via conditional compilation to avoid overlapping behavior.
- Keep route definitions **predictable and version-scoped**:

```csharp
var v1 = app.MapGroup("/v1")
	.WithApiVersionSet(v1Set);

var v2 = app.MapGroup("/v2")
	.WithApiVersionSet(v2Set);

v1.MapGet("/users", ...); // v1 definition
v2.MapGet("/users", ...); // v2 definition
```

> 💡 Tip: During version migration, prefer explicit registration over inferred routing. A mismatch between endpoints and their API version metadata may lead to incomplete OpenAPI documents or user-facing inconsistencies.

### 🧷 Use `AssumeDefaultVersionWhenUnspecified = true` wisely

ASP.NET Core API Versioning allows you to set `AssumeDefaultVersionWhenUnspecified = true`, which routes incoming requests to a fallback version when no explicit version is provided. While helpful for backward compatibility, this option can also introduces silent routing behaviors that are difficult to detect.

#### ⚠️ Gotchas to watch for

- Requests lacking a version may unexpectedly hit **deprecated** or **outdated** endpoints if the default isn’t kept in sync.
- If you promote `v2` but leave `v1` as the default, version-less clients/requests may continue consuming legacy logic even if `v2` is intended as the current target.
- OpenAPI documents do not encode fallback routing behavior. Consumers inspecting version-specific specs may be unaware that unversioned requests are routed to a fallback version via `AssumeDefaultVersionWhenUnspecified = true`. Make this behavior explicit through endpoint summaries, release notes, or your external API documentation.

#### ✅ Best Practices

- Set the default version thoughtfully based on lifecycle stage, not simply the oldest version:

```csharp
options.DefaultApiVersion = new ApiVersion(2, 0); // ✅ Prefer current default
options.AssumeDefaultVersionWhenUnspecified = true;
```

- Since OpenAPI documents are scoped per version, fallback routing behavior is not encoded in the specification. Consumers inspecting version-specific specs may assume version declarations are required, even if `AssumeDefaultVersionWhenUnspecified = true` is enabled. To avoid ambiguity, clarify fallback logic through endpoint remarks, release notes, or external documentation.
- Include `ReportApiVersions = true` to expose available and deprecated versions in response headers.
- If multiple consumers rely on different versions, consider enforcing version declaration via validation middleware instead of assuming defaults silently.

> 💡 Tip: A default version is not just a technical fallback, it's a statement of support. Treat it like part of your contract, and revisit it every time you publish a new version.

## 📚 References

- [Official ASP.NET Core Web API documentation](https://learn.microsoft.com/aspnet/core/web-api/?view=aspnetcore-9.0)
- [API Versioning in ASP.NET Core (Official Docs)](https://learn.microsoft.com/aspnet/core/web-api/advanced/api-versioning?view=aspnetcore-9.0)
- [DotNet/AspNet-Api-Versioning Wiki](https://github.com/dotnet/aspnet-api-versioning/wiki)
- [Keyed Dependency Injection in .NET 8+](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection-usage#keyed-and-named-service-registration)
- [Minimal APIs in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-9.0)
- [OpenAPI support in ASP.NET Core](https://learn.microsoft.com/aspnet/core/web-api/advanced/openapi?view=aspnetcore-9.0)
- [Swashbuckle.AspNetCore (Swagger for .NET)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [Scalar: OpenAPI UI for .NET](https://scalar.com/docs/aspnetcore)
- [Problem Details for HTTP APIs (RFC 7807)](https://datatracker.ietf.org/doc/html/rfc7807)

---

**_🧭 Stay Curious. Build Thoughtfully._**