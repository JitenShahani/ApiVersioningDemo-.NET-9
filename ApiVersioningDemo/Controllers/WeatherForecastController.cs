namespace ApiVersioningDemo.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route ("[controller]")]
[Tags ("Weather Forecast")]
public class WeatherForecastController : ControllerBase
{
	private static readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

	[HttpGet (Name = "GetWeatherForecast")]
	[EndpointSummary ("Weather Forecast")]
	[EndpointDescription ("This endpoint get's next 5 days weather forecast")]
	public IEnumerable<WeatherForecast> Get ()
	{
		return [..Enumerable
			.Range (1, 5)
			.Select (index => new WeatherForecast
			{
				Date = DateOnly.FromDateTime (DateTime.Now.AddDays (index)),
				TemperatureC = Random.Shared.Next (-20, 55),
				Summary = _summaries[Random.Shared.Next (_summaries.Length)]
			})];
	}
}