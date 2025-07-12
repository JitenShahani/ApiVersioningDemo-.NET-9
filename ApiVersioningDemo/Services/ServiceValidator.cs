namespace ApiVersioningDemo.Services;

public class ServiceValidator
{
	private readonly IEmployee _employeeService;
	private readonly IEmployee _tempEmployeeService;

	public ServiceValidator (
		[FromKeyedServices ("employeeService")] IEmployee employeeService,
		[FromKeyedServices ("tempEmployeeService")] IEmployee tempEmployeeService) =>
			(_employeeService, _tempEmployeeService) = (employeeService, tempEmployeeService);
}