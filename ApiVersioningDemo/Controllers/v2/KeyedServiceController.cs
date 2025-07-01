namespace ApiVersioningDemo.Controllers.v2;

[ApiController]
[Route ("api/v{version:apiVersion}/[controller]")]
[ApiVersion ("2.0")]
[Tags ("Keyed Services")]
public class KeyedServiceController : ControllerBase
{
	private readonly IEmployee _employeeRepository;

	public KeyedServiceController ([FromKeyedServices ("employeeRepo")] IEmployee employeeRepository)
	{
		_employeeRepository = employeeRepository;
	}

	[HttpGet ("employee")]
	[ProducesResponseType<Response> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Employee")]
	[EndpointDescription ("This endpoint get's the message from the Employee keyed service.")]
	public IActionResult GetEmployee ()
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		var result = new Response
		{
			Message = $"{_employeeRepository.GetMessage ()} - {version}"
		};

		return Ok (result);
	}

	[HttpGet ("tempEmployee")]
	[ProducesResponseType<Response> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Temporary Employee")]
	[EndpointDescription ("This endpoint get's the message from the Temporary Employee keyed service.")]
	public IActionResult GetTempEmployee ([FromKeyedServices ("tempEmployeeRepo")] IEmployee employeeRepository)
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		var result = new Response
		{
			Message = $"{employeeRepository.GetMessage ()} - {version}"
		};

		return Ok (result);
	}

	[HttpGet ("both")]
	[ProducesResponseType<Response[]> (StatusCodes.Status200OK, "application/json")]
	[EndpointSummary ("Get Employees")]
	[EndpointDescription ("This endpoint get's the message from both Employee & Temporary Employee keyed services.")]
	public IActionResult GetBoth ([FromKeyedServices ("tempEmployeeRepo")] IEmployee tempEmployeeRepository)
	{
		var version = "v" + HttpContext.GetRequestedApiVersion ();

		Response empRepoMessage = new () { Message = $"{_employeeRepository.GetMessage ()} - {version}" };
		Response tempRepoMessage = new () { Message = $"{tempEmployeeRepository.GetMessage ()} - {version}" };

		Response[] responses = [empRepoMessage, tempRepoMessage];

		return Ok (responses);
	}

	[HttpGet ("Employees")]
	[ProducesResponseType<List<Employees>> (StatusCodes.Status200OK, "application/json")]
	[ProducesResponseType (StatusCodes.Status400BadRequest)]
	[EndpointSummary ("List of Employees")]
	[EndpointDescription ("This endpoint get's the list of employees from the database.")]
	public IActionResult GetEmployees ()
	{
		int randomNumber = Random.Shared.Next (0, 2);

		// Populate Problem Details just in case bad request is sent.
		ProblemDetailsFactory problemDetailsFactory = HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory> ();
		ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails (HttpContext, StatusCodes.Status400BadRequest);

		problemDetails.Extensions["errors"] = new Dictionary<string, object?> ()
		{
			["Message"] = "The request was randomly rejected to simulate a Bad Request scenario."
		};

		CustomProblemDetails customProblemDetails = new ()
		{
			Type = problemDetails.Type,
			Title = problemDetails.Title,
			Status = problemDetails.Status,
			Detail = problemDetails.Detail,
			Instance = problemDetails.Instance,
			TraceId = problemDetails.Extensions["traceId"]!.ToString (),
			RequestId = problemDetails.Extensions["requestId"]!.ToString (),
			Errors = problemDetails.Extensions["errors"] as Dictionary<string, object?>
		};

		return randomNumber == 1
			? Ok (new List<Employees>
			{
				new () { FirstName = "Durgesh", LastName = "Shukla", Age = 48 },
				new () { FirstName = "Dhruv", LastName = "Trivedi", Age = 51 },
				new () { FirstName = "Jiten", LastName = "Shahani", Age = 49 },
				new () { FirstName = "Rahul", LastName = "Pal", Age = 32 }
			})
			: BadRequest (customProblemDetails);
	}
}