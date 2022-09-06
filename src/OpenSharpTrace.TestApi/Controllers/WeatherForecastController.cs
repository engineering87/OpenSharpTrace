using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private readonly ILogger<WeatherForecastController> _logger;

        // OpenSharpTrace integration for ISqlTraceRepository

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger, 
            ISqlTraceRepository repository) : base(logger, repository)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/Get")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("/GetBadRequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("");
        }

        [HttpGet]
        [Route("/GetException")]
        public IActionResult GetException()
        {
            throw new Exception("Generic exception");
        }
    }
}
