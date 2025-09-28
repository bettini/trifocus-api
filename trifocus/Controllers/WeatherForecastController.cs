using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace trifocus.Controllers;

/// <response code="401">User is not authenticated</response>
/// <response code="403">User is not authorized</response>
[ApiController]
[Route("[controller]")]
[Authorize]  // Requires authentication for all actions
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [Authorize(Policy = "RequireAthleteRole")]  // Only athletes can access this endpoint
    /// <response code="200">Returns the weather forecast data</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WeatherForecast>))]
    public IEnumerable<WeatherForecast> Get()
    {
        var userName = User.Identity?.Name ?? "unknown";
        _logger.LogInformation("Weather forecast requested by user: {User}", userName);
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    /// <response code="200">Returns admin-only data</response>
    [HttpGet("admin")]
    [Authorize(Policy = "RequireAdminRole")]  // Only admins can access this endpoint
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAdminData()
    {
        return Ok(new { message = "This is admin-only data" });
    }
}
