using Microsoft.AspNetCore.Mvc;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Controllers;

namespace OpenSharpTrace.TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : OpenSharpTraceController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger _logger;

        // TODO spostare la DI del ISqlTraceRepository solo all'interno del controller OpenSharpTraceController

        public WeatherForecastController(
            ILoggerFactory loggerFactory, 
            ISqlTraceRepository repository) : base(loggerFactory, repository)
        {
            _logger = loggerFactory.CreateLogger(GetType().ToString());
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [HttpGet(Name = "GetBadRequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("");
        }

        [HttpGet]
        [HttpGet(Name = "GetException")]
        public IActionResult GetException()
        {
            throw new Exception("Generic exception");
        }
    }
}