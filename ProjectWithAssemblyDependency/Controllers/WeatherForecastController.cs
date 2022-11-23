using Microsoft.AspNetCore.Mvc;
using SimpleWebApi;

namespace ProjectWithAssemblyDependency.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController: ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get() => throw new NotImplementedException();

    [HttpPost(Name = "PostWeatherForecastForCity")]
    public WeatherForecast PostForCity(string cityName) => throw new NotImplementedException();

    [HttpGet(Name = "Cities")]
    public List<string> Cities(string cityName) => throw new NotImplementedException();
}
