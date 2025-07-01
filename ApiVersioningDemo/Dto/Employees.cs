namespace ApiVersioningDemo.Dto;

public class Employees
{
	public Guid Id { get; set; } = Guid.CreateVersion7 ();
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public int Age { get; set; }
}