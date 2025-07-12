<!--
## 📚 Table of Contents

- [🔄 API Versioning Demo](#-api-versioning-demo)
- [🎯 Key Objectives](#-key-objectives)
- [📂 Project Structure](#-project-structure)
- [🌐 Launch Profiles & UI Clients](#-launch-profiles--ui-clients)
- [📌 Endpoints](#-endpoints)
- [🔢 API Versioning Strategies](#-api-versioning-strategies)
	- [1. ❓ Query Parameter Versioning](#1--query-parameter-versioning)
	- [2. 📬 Header-Based Versioning](#2--header-based-versioning)
	- [3. 📰 Media Type Versioning](#3--media-type-versioning)
	- [4. 🌐 URL Segment Versioning (Used in this Project)](#4--url-segment-versioning-used-in-this-project)
- [⚙️ Configuration Notes](#-configuration-notes)
	- [🧩 IoC Configuration & Registration](#-ioc-configuration--registration)
		- [🛡️ Validation Behavior](#-validation-behavior)
		- [📘 Controller & Serialization Setup](#-controller--serialization-setup)
		- [🩺 Problem Details Configuration](#-problem-details-configuration)
		- [📚 OpenAPI & Documentation](#-openapi--documentation)
		- [🔑 Keyed DI & Validation](#-keyed-di--validation)
	- [🧾 DTOs](#-dtos)
		- [Response.cs](#-responsecs)
		- [WeatherForecast.cs](#weatherforecastcs)
	- [🛠️ Services](#-services)
		- [📄 IEmployee.cs](#-iemployeecs)
		- [👤 EmployeeService.cs](#-employeeservicecs)
		- [⛑️ TempEmployeeService.cs](#-tempemployeeservicecs)
		- [✅ ServiceValidator.cs](#-servicevalidatorcs)
	- [🔧 API Versioning Setup](#-api-versioning-setup)
		- [🧭 How to Configure API Versioning](#-how-to-configure-api-versioning)
		- [🧬 Generate Version-Specific Documents](#-generate-version-specific-documents)
			- [🔹 Configure Using OpenApi](#-configure-using-openapi)
			- [🔹 Configure Using Swashbuckle](#-configure-using-swashbuckle)
		- [📘 UI Client Integration Overview](#-ui-client-integration-overview)
			- [🛠️ Configure Swagger UI](#-configure-swagger-ui)
			- [🛠️ Configure Scalar](#-configure-scalar)
		- [🔎 Versioned Endpoint Samples](#-versioned-endpoint-samples)
			- [📂 Controller-Based Endpoints](#-controller-based-endpoints)
			- [📂 Minimal API Endpoints](#-minimal-api-endpoints)
	- [🧠 Hidden Gotchas & Best Practices](#-hidden-gotchas--best-practices)
- [📚 References](#-references)
- [🧭 Stay Curious. Build Thoughtfully.](#_stay-curious-build-thoughtfully-)
-->

# 🔄 API Versioning Demo

[![Asp.Versioning.Mvc](https://img.shields.io/nuget/dt/Asp.Versioning.Mvc.svg?label=Asp.Versioning.Mvc&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Asp.Versioning.Mvc/)
[![Asp.Versioning.Mvc.ApiExplorer](https://img.shields.io/nuget/dt/Asp.Versioning.Mvc.ApiExplorer.svg?label=Asp.Versioning.Mvc.ApiExplorer&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Asp.Versioning.Mvc.ApiExplorer/)
[![Microsoft.AspNetCore.OpenApi](https://img.shields.io/nuget/dt/Microsoft.AspNetCore.OpenApi.svg?label=Microsoft.AspNetCore.OpenApi&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/)
[![Scalar.AspNetCore](https://img.shields.io/nuget/dt/Scalar.AspNetCore.svg?label=Scalar.AspNetCore&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Scalar.AspNetCore/)
[![Swashbuckle.AspNetCore](https://img.shields.io/nuget/dt/Swashbuckle.AspNetCore.svg?label=Swashbuckle.AspNetCore&style=flat-square&logo=Nuget)](https://www.nuget.org/packages/Swashbuckle.AspNetCore/)

This repository demonstrates how to implement **API Versioning** and **Keyed Singleton Services** in an ASP.NET Core Web API using **.NET 9**. It features both **Minimal APIs** and **Controller-based endpoints**, showing how to structure versioned routing, resolve services via constructor injection, and configure flexible, version-aware API documentation using **Swagger UI** and **Scalar**.

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
│   │   └── KeyedServiceController.cs		v1 controller (deprecated)
│   ├── v2/
│   │   └── KeyedServiceController.cs		v2 controller
│   └── WeatherForecastController.cs		Version-neutral controller
├── Dto/
│   ├── Response.cs				DTO for API responses
│   └── WeatherForecast.cs			DTO for Weather forecast
├── MinimalEndpoints/
│   ├── MyEndpoints.cs				Minimal Endpoints with both v1 & v2
├── Services/
│   ├── IEmployee.cs				Employee Service interface
│   ├── EmployeeService.cs			Main Employee service
│   ├── TempEmployeeService.cs			Temporary Employee service
│   └── ServiceValidator.cs			Validates that all required keyed IEmployee services are registered at startup.
│						This "dummy" service is injected with both `employeeRepo` and `tempEmployeeRepo` keyed services.
│						This ensures the DI container throws an error during app build if any are missing or misconfigured.
│						This provides early, fail-fast validation of your dependency injection setup.
├── Startup/
│   ├── IoC.cs					Dependency injection and service registration
│   └── Middleware.cs				Middleware and pipeline config
└── Swagger/					These classes are no longer used in this project but are included for future reference.
    ├── ConfigureSwaggerOptions.cs		Swashbuckle/OpenAPI configuration
    └── SwaggerDefaultValues.cs			Swashbuckle/OpenAPI operation filter
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

- The default query string key is `api-version`, but it can be changed (e.g., new QueryStringApiVersionReader("version")).
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
- Requires route configuration on controllers, e.g. `[Route ("api/{version:apiVersion}/[controller]")]`.
- Enhances discoverability and works seamlessly with [ApiVersion], version-specific DI, and OpenAPI grouping.

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

> These registrations prepare the application to expose OpenAPI-compatible metadata and endpoint descriptions. `AddOpenApi()` adds baseline support for spec generation and is typically used in conjunction with tools like Swagger/Scalar.

#### 🔑 Keyed DI & Validation

```csharp
builder.Services.AddKeyedSingleton<IEmployee, EmployeeService>("employeeService");
builder.Services.AddKeyedSingleton<IEmployee, TempEmployeeService>("tempEmployeeService");
builder.Services.AddSingleton<ServiceValidator>();
```

Registers two implementations of `IEmployee` using keyed dependency injection, allowing versioned controllers or minimal APIs to request specific services via `[FromKeyedServices("...")]`. This pattern avoids conditional resolution logic and supports clean separation of behavior across versions. The `ServiceValidator` class is a dummy consumer of both keyed services injected at startup to trigger container validation. Combined with `.ValidateOnBuild`, it ensures all required dependencies are registered before runtime.

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

#### ✅ `ServiceValidator.cs`

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

This section demonstrates how to configure API Versioning and integrate it with Swagger and Scalar interfaces for version-aware documentation and routing.

#### 🧭 How to Configure API Versioning

API Versioning in this project is powered by `Asp.Versioning.Mvc` and its companion `Asp.Versioning.Mvc.ApiExplorer`. These packages provide endpoint version routing, controller tagging via `[ApiVersion(...)]`, and OpenAPI grouping support for Swagger/Scalar documentation.

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

| Feature								| Purpose																								|
|---------------------------------------|-------------------------------------------------------------------------------------------------------|
| `DefaultApiVersion`					| Declares v2.0 as the supported default version.														|
| `AssumeDefaultVersionWhenUnspecified`	| Enables fallback behavior for unversioned requests.													|
| `ReportApiVersions`					| Adds version headers like `api-supported-versions` and `api-deprecated-versions` to API responses.	|
| `UrlSegmentApiVersionReader`			| Parses version from URL segments (e.g., `/api/v2/...`).												|
| `VersionByNamespaceConvention`		| Tags controllers with `[ApiVersion]` based on their folder namespace.									|
| `AddApiExplorer`						| Groups versioned endpoints for OpenAPI, Swagger/Scalar generation.									|

Here's how to configure Controllers/Controller Actions and Minimal endpoints to implement API Versioning.

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

This configuration ensures that all endpoints whether controllers or minimal APIs are version-aware, grouped correctly in your documentation interfaces, and gracefully handle fallback scenarios. The result is a clean, discoverable, and scalable API structure.

#### 🧬 Generate Version-Specific Documents

The configuration under `Configure API Versioning` is sufficient for internal routing, version metadata tagging, and endpoint grouping. However, if you intend to visualize APIs through UI clients like `Swagger` or `Scalar`, you'll need to configure version-specific OpenAPI documents.

ASP.NET Core supports two options for generating versioned documentation:

| Approach			| Trade-offs																								|
|-------------------|-----------------------------------------------------------------------------------------------------------|
| AddOpenApi()		| Deprecated endpoints do not show correct metadata unless explicitly configured via transformers.		|
| AddSwaggerGen()	| Since Swagger fully owns the document generation process, all configuration even if already set up in `AddOpenApi()` must be **manually redefined**, including authentication, filters, and schema options. |

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

✅ This setup registers versioned OpenAPI documents using `AddOpenApi(...)`, configured to serve them at `/openApi/v1.json`, `/openApi/v2.json`, etc. Paths are standardized via middleware configuration. These documents contain grouped endpoints and metadata per version used by UI clients like Swagger or Scalar.

##### 🔹 Configure Using Swashbuckle

```csharp
// IoC.cs
builder.Services.AddOpenApi();

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

✅ This configuration uses `AddSwaggerGen(...)` to generate versioned OpenAPI documents. These are served at `/openApi/v1.json`, `/openApi/v2.json`, matching the same route template. Any existing metadata or settings from AddOpenApi() do not apply. Swashbuckle owns the generation process and must be re-configured independently.

#### 📘 UI Client Integration Overview

Once versioned OpenAPI documents are registered, they can be rendered using UI clients like `Swagger` or `Scalar`. These integrations allow developers to explore grouped endpoints, inspect metadata, and test versioned APIs interactively.

The following sections walks through configuration for each UI client.

##### 🛠️ Configure Swagger UI

Swagger UI enables interactive exploration of your versioned API specs. Once OpenAPI documents are served at known paths, you can configure the UI for optimal readability and developer experience.

```csharp
// Middleware.cs
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

🧩 Scalar renders versioned documentation automatically, using the keys registered in the pipeline. It groups endpoints per version and displays embedded metadata such as titles, descriptions, and deprecation tags. No visual customization is required. The UI adapts to document content and reflects it cleanly. Scalar’s minimalist layout offers immediate access to endpoints, tags, request details, and response models, ideal for clean client-side exploration.

#### 🔎 Versioned Endpoint Samples

Here’s how versioned endpoints are declared using both `Controllers` and `Minimal APIs`, showing how `[ApiVersion]`, `GroupName`, and route templates coordinate to ensure version-aware behavior across routing and documentation.

##### 📂 Controller-Based Endpoints

```csharp
// KeyedServiceController.V1

[ApiController]
[Route ("api/v{version:apiVersion}/[controller]")]
[ApiExplorerSettings (GroupName = "v1")]
[ApiVersion ("1.0", Deprecated = true)]
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

✅ Controllers define both `[ApiVersion(...)]` and `GroupName` to register their endpoints in version-specific documentation. Routing relies on a URL segment pattern: api/{version:apiVersion}/... Each action returns a typed DTO and uses FromKeyedServices for clean DI resolution.

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
```

✅ Minimal APIs rely on `ApiVersionSet` for declarative versioning. Use `.NewApiVersionSet()` to build a set, then bind it to endpoints with `.WithApiVersionSet(...)`. Metadata like `.HasApiVersion(...)` and `.WithOpenApi()` ensures correct grouping and discovery in tools like Swagger or Scalar. Minimal APIs integrate cleanly with both Swagger and Scalar, provided they’re routed and grouped consistently.

## 🧠 Hidden Gotchas & Best Practices

Here are a few nuanced tips to keep your versioned API setup clean, maintainable, and discoverable:

- 🧩 Keep [ApiVersion] consistent with declared JSON docs. Ensure that your registered OpenAPI documents match the version values used in controller and minimal endpoints. Mismatches lead to invisible routes.
- ⚠️ Don’t forget `ApiExplorerSettings(GroupName = "vX")`. Omitting this leads to missing endpoints in Swagger and Scalar UI. Grouping is explicit.
- ⚠️ Avoid duplicate route definitions across versions If two controllers define the same action under the same route but different versions, only one may be rendered if grouping isn’t handled correctly.
- 🛠️ Minimal APIs must use `WithApiVersionSet(...)` for proper registration. Without it, endpoints won’t be grouped correctly in OpenAPI documents, even if they declare a version.

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
