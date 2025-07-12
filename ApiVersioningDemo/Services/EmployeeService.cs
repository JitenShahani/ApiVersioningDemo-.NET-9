namespace ApiVersioningDemo.Services;

public class EmployeeService : IEmployee
{
	public string GetMessage () => "Hello from Employee Service";
}