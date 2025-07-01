namespace ApiVersioningDemo.Services;

public class ServiceValidator
{
	private readonly IEmployee _employeeRepository;
	private readonly IEmployee _tempEmployeeRepository;

	public ServiceValidator (
		[FromKeyedServices ("employeeRepo")] IEmployee employeeRepository,
		[FromKeyedServices ("tempEmployeeRepo")] IEmployee tempEmployeeRepository) =>
			(_employeeRepository, _tempEmployeeRepository) = (employeeRepository, tempEmployeeRepository);
}