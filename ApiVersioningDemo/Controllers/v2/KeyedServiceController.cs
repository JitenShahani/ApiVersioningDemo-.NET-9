﻿namespace ApiVersioningDemo.Controllers.V2;

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