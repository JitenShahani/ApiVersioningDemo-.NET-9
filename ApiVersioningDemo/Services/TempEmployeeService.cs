namespace ApiVersioningDemo.Services;

public class TempEmployeeService : IEmployee
{
	public string GetMessage () => "Hello from Temporary Employee Service";
}