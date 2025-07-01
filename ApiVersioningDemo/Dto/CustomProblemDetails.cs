namespace ApiVersioningDemo.Dto;

public class CustomProblemDetails
{
	public string? Type { get; set; }
	public string? Title { get; set; }
	public int? Status { get; set; }
	public string? Detail { get; set; }
	public string? Instance { get; set; }
	public string? TraceId { get; set; }
	public string? RequestId { get; set; }
	public Dictionary<string, object?>? Errors { get; set; }
}