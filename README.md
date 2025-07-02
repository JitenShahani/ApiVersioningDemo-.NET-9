# 🔄 API Versioning in .NET 9

This project demonstrates how to implement **API Versioning** and **Keyed Services** in a controller-based ASP.NET Core Web API using **.NET 9**. It also integrates both **Swagger** and **Scalar** UIs to provide flexible, version-aware API documentation.

---

## 🚀 Features & Highlights

- ✅ Built with **.NET 9** using controller-based architecture for version-aware APIs.
- 🔑 Showcases **Keyed Services** with multiple `IEmployee` implementations using `.AddKeyedSingleton()`.
- 🔢 Implements **API Versioning** via [`Asp.Versioning.Mvc`](https://github.com/dotnet/aspnet-api-versioning) with `[ApiVersion]` attributes and URL segment routing.
- 📚 Leverages the [OpenAPI Specification](https://www.openapis.org) to generate interactive, versioned API documentation.
- 🧭 Supports dual UI rendering via [Swagger UI](https://swagger.io) and [Scalar](https://scalar.com/) for modern API exploration.
- 🛠️ Includes fail-fast startup validation with `ServiceValidator.cs` to ensure proper DI configuration.
- ✨ Emphasizes clear structure, clean separation of concerns, and educational readability.

---

## 📂 Project Structure

```
Controllers/
├── v1/KeyedServiceController.cs	v1 controller (deprecated)
├── v2/KeyedServiceController.cs	v2 controller
└── WeatherForecastController.cs	Version-neutral controller
Dto/
├── CustomProblemDetails.cs		DTO for Problem Details. You can use the built-in `ProblemDetails` class, but this custom DTO follows the Property naming convention configured in IoC container. Property names will be printed using Pascal Case.
├── Employees.cs			DTO for employee
├── Response.cs				DTO for API responses
└── WeatherForecast.cs			DTO for weather forecast
Services/
├── IEmployee.cs			Employee service interface
├── EmployeeRepository.cs		Main employee repository
├── TempEmployeeRepository.cs		Temporary employee repository
└── ServiceValidator.cs			Validates that all required keyed IEmployee services are registered at startup.
					This "dummy" singleton is injected with both `employeeRepo` and `tempEmployeeRepo` keyed services, ensuring the DI container throws an error during app build if any are missing or misconfigured.
					This provides early, fail-fast validation of your dependency injection setup.
Startup/
├── IoC.cs				Dependency injection and service registration
└── Middleware.cs			Middleware and pipeline config
Swagger/
├── ConfigureSwaggerOptions.cs		Swagger/OpenAPI configuration
└── SwaggerDefaultValues.cs		Swagger operation filter
```

---

## 🌐 Launch Profiles & UIs
The current launch settings is configured to dynamically load one of the following UIs:

| Launch Profile           | API Explorer Loaded |
|--------------------------|---------------------|
| HTTP                     | Swagger UI          |
| HTTPS / IIS Express      | Scalar UI           |

> **Tip:** You can change the launch profile by editing `Properties/launchSettings.json`.

---

## 📌 Endpoints

| Version			| Route									| Description								|
|-------------------|---------------------------------------|-------------------------------------------|
| v1 (Deprecated)	| GET /api/v1/keyedservice/employee		| Returns Employee message (v1)				|
| v1 (Deprecated)	| GET /api/v1/keyedservice/tempEmployee	| Returns Temp Employee message (v1)		|
| v1 (Deprecated)	| GET /api/v1/keyedservice/both			| Returns both Employee types (v1)			|
| v2				| GET /api/v2/keyedservice/employee		| Returns Employee message (v2)				|
| v2				| GET /api/v2/keyedservice/tempEmployee	| Returns Temp Employee message (v2)		|
| v2				| GET /api/v2/keyedservice/both			| Returns both Employee types (v2)			|
| v2				| GET /api/v2/keyedservice/Employees	| Returns list of employees or error (v2)	|
| Version neutral   | GET /WeatherForecast                  | Returns next 5 days weather forecast      |

---

## 🧠 Key Internals Explained

- **🔢 API Versioning & OpenAPI Documentation**  

	API versioning is implemented using [`Asp.Versioning.Mvc`](https://github.com/dotnet/aspnet-api-versioning), which routes requests via `UrlSegmentApiVersionReader` (e.g., `/api/v1/...`) and annotates controllers using `[ApiVersion(...)]`. This version metadata flows into ASP.NET Core's API Explorer to generate OpenAPI documents, one per version, under `/openApi/{version}.json`. These specs are then rendered using [Swagger UI](https://swagger.io) and [Scalar](https://scalar.com/), providing interactive, version-aware API documentation.

- **🔑 Keyed Service Injection**  

 	Both `EmployeeRepository` and `TempEmployeeRepository` implement `IEmployee`, and are registered as keyed singletons. They’re injected into versioned controllers using `[FromKeyedServices("key")]` enabling precise, version-specific DI resolution without conditionals.
- **🧪 Fail-Fast Service Validation**  

 	Validates service resolution at startup using `ServiceValidator.cs` and `.ValidateOnBuild = true`, ensuring that missing or misconfigured dependencies are caught early with clear exceptions during `app.Build()`.
- **🧱 Middleware & Formatting Pipeline**  

 	Middleware is organized via `Startup/IoC.cs` and `Startup/Middleware.cs`, which configures JSON serialization, basic exception handling with `CustomProblemDetails`, and developer-friendly diagnostics.

---

## 📘 Explore & Learn

This project is designed to help developers explore advanced ASP.NET Core features like Keyed Services and API Versioning.

- 💡 Use it as a sandbox for experimentation
- 🔍 Dive into version-aware controller patterns
- 🛠️ Study the structure, middleware, and OpenAPI integration

---
**_🧭 Stay Curious. Build Thoughtfully._**
