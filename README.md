# 🔄 API Versioning & Keyed Services in .NET 9

This project demonstrates how to implement **API Versioning** and **Keyed Services** in a controller-based ASP.NET Core Web API using **.NET 9**. It also integrates both **Swagger** and **Scalar** UIs to provide flexible, version-aware API documentation.

---

## 🎯 Project Highlights

- ✅ Register and resolve multiple service implementations using `.AddKeyedSingleton()`
- 🔢 Implement API versioning with [`Asp.Versioning.Mvc`](https://github.com/dotnet/aspnet-api-versioning)
- 🌐 Provide both **Swagger** and **Scalar** UIs for exploring endpoints
- 🧼 Maintain clean, versioned controller structure with flexible endpoint resolution

---

## 🚀 Features

- **.NET 9**: Built with the latest .NET 9 framework.
- **API Versioning**: Multiple API versions using [Asp.Versioning.Mvc](https://github.com/dotnet/aspnet-api-versioning).
- **OpenAPI**: Industry-standard specification for describing RESTful APIs in a machine-readable format. Learn more at [OpenAPI Specification](https://www.openapis.org).
  - **Swagger**: Interactive API documentation with [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
  - **Scalar**: Modern, customizable API reference UI with [Scalar](https://scalar.com/).

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

> **Tip:** You can change the launch profile by editing `Properties\launchSettings.json`.

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

## 📤 Key Concepts in Action

- **Keyed Services**:
    - The `IEmployee` interface is implemented by both `EmployeeRepository` and `TempEmployeeRepository`.
    - Services are registered using `.AddKeyedSingleton()` and resolved in controllers via `[FromKeyedServices("key")]`, allowing precise injection based on the required implementation and API version.
- **API Versioning**:
    - Controllers are versioned using attributes like `[ApiVersion("1.0", Deprecated = true)]` for v1 and `[ApiVersion("2.0")]` for v2.
    - The API version is specified in the URL using `UrlSegmentApiVersionReader`, enabling routes such as `/api/v1/...` and `/api/v2/...`.
- **Swagger + Scalar**:
    - Swagger documentation is generated and grouped by API version, accessible via `/swagger/index.html`.
    - The Scalar UI is enabled only for HTTPS/IIS Express launch profiles, providing a modern, version-aware API reference experience. You can access it at `/scalar`.

---

## 🔧 Key Components

- **🔢 API Versioning**  
  Powered by [`Asp.Versioning.Mvc`](https://github.com/dotnet/aspnet-api-versioning), the project uses `UrlSegmentApiVersionReader` to route versioned requests (e.g., `/api/v1/...`, `/api/v2/...`). Controllers are annotated with `[ApiVersion]`, with v1 explicitly marked as deprecated.
- **🔑 Keyed Services**  
  The `IEmployee` abstraction is implemented by both `EmployeeRepository` and `TempEmployeeRepository`, registered via `.AddKeyedSingleton()`. Services are injected using `[FromKeyedServices("key")]`, enabling precise DI resolution per controller or API version.
- **🧪 Service Validator**  
  The `ServiceValidator` class is registered to ensure all required keyed services are resolved during application startup. This proactive check prevents misconfiguration and enforces a fail-fast pattern.
- **📚 OpenAPI Support**  
  OpenAPI documentation is generated using [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore), with UI rendering via both Swagger and [Scalar](https://scalar.com/). Endpoints are grouped by version and served from `/openApi/{version}.json`.
- **🧱 Middleware & Pipeline**  
  Centralized configuration via `Startup/Middleware.cs` handles routing, JSON formatting (PascalCase), error responses using `CustomProblemDetails`, and exposes detailed version-aware diagnostics in dev mode.

---

## 📘 Explore & Learn

This project is designed to help developers explore advanced ASP.NET Core features like Keyed Services and API Versioning.

- 💡 Use it as a sandbox for experimentation
- 🔍 Dive into version-aware controller patterns
- 🛠️ Study the structure, middleware, and OpenAPI integration