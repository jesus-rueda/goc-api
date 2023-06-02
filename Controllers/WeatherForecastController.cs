using Microsoft.AspNetCore.Mvc;

namespace goc_api.Controllers
{
    using System.Data.SqlClient;

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly string myConnectionString;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config)
        {
            _logger = logger;
            this.myConnectionString = config.GetConnectionString("db");
        }


        [HttpGet]
        [Route("/pingdb")]
        public IEnumerable<string> PingDb()
        {
            using var conn = new SqlConnection(this.myConnectionString);
            using var command = conn.CreateCommand();
            command.CommandText = "SELECT name FROM sys.tables";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return reader.GetString(0);
            }
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
    }

   
}