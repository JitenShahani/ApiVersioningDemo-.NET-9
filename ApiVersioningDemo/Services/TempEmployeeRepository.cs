namespace ApiVersioningDemo.Services;

public class TempEmployeeRepository : IEmployee
{
	public string GetMessage () => "Hello from Temporary Employee Repository";
}