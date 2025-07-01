namespace ApiVersioningDemo.Controllers.v1;

[ApiController]
[Route ("api/v{version:apiVersion}/[controller]")]
[ApiVersion ("1.0", Deprecated = true)]
[Tags ("Keyed Services")]
[Obsolete ("Deprecated")]
public class KeyedServiceController : ControllerBase
{
	private readonly IEmployee _employeeRepository;

	public KeyedServiceController ([FromKeyedServices ("employeeRepo")] IEmployee employeeRepository) =>
		_employeeRepository = employeeRepository;

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
	[ProducesResponseType<Response[]> (StatusCodes.Status200OK, "application/json")]
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
		Response tempEmpRepoMessage = new () { Message = $"{tempEmployeeRepository.GetMessage ()} - {version}" };

		Response[] responses = [empRepoMessage, tempEmpRepoMessage];

		return Ok (responses);
	}
}