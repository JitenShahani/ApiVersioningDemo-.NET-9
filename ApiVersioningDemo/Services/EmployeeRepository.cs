namespace ApiVersioningDemo.Services;

public class EmployeeRepository : IEmployee
{
	public string GetMessage () => "Hello from Employee Repository";
}