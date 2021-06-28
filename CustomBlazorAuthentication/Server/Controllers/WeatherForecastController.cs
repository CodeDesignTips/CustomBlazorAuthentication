using CustomBlazorAuthentication.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Server.Controllers
{
    [Route("[controller]")]
    public class WeatherForecastController : BaseController
    {
        #region Private members
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public WeatherForecastController()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Get the weather forecast info
        /// </summary>
        /// <returns>weather forecast info</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
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
        #endregion
    }
}
