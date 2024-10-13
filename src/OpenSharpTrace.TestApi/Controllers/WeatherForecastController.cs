using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Controllers;
using OpenSharpTrace.Persistence.SQL.Entities;
using OpenSharpTrace.TraceQueue;
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

        private static readonly List<WeatherForecast> WeatherForecasts = new List<WeatherForecast>();

        // OpenSharpTrace integration for ITraceQueue

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            ITraceQueue<Trace> transactionQueue) : base(logger, transactionQueue)
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
        [Route("/GetById/{id}")]
        public IActionResult GetById(int id)
        {
            if (id < 0 || id >= WeatherForecasts.Count)
            {
                return NotFound("Forecast not found.");
            }

            return Ok(WeatherForecasts[id]);
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

        [HttpPost]
        [Route("/Add")]
        public IActionResult Add([FromBody] WeatherForecast forecast)
        {
            if (forecast == null)
            {
                return BadRequest("Invalid forecast data.");
            }

            WeatherForecasts.Add(forecast);
            return CreatedAtAction(nameof(GetById), new { id = WeatherForecasts.Count - 1 }, forecast);
        }

        [HttpPut]
        [Route("/Update/{id}")]
        public IActionResult Update(int id, [FromBody] WeatherForecast forecast)
        {
            if (id < 0 || id >= WeatherForecasts.Count || forecast == null)
            {
                return BadRequest("Invalid forecast data or ID.");
            }

            WeatherForecasts[id] = forecast;
            return NoContent(); // 204 No Content
        }

        [HttpDelete]
        [Route("/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (id < 0 || id >= WeatherForecasts.Count)
            {
                return NotFound("Forecast not found.");
            }

            WeatherForecasts.RemoveAt(id);
            return NoContent(); // 204 No Content
        }
    }
}
