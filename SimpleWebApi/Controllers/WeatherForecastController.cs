using Microsoft.AspNetCore.Mvc;

namespace SimpleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController: ControllerBase
{
    public WeatherForecastController(ILogger<WeatherForecastController> logger) { }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get() => throw new NotImplementedException();

    [HttpPost(Name = "PostWeatherForecastForCity")]
    public WeatherForecast PostForCity(string cityName) => throw new NotImplementedException();
}
