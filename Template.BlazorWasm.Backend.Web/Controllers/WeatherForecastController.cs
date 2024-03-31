namespace Template.BlazorWasm.Backend.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

using Template.BlazorWasm.Api;

[ApiController]
[Route("[controller]")]
public sealed class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
#pragma warning disable CA5394
        return Enumerable.Range(1, 5).Select(static index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
#pragma warning restore CA5394
    }
}
