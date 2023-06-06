using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Goc.Api;
    using Goc.Api.Hubs;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHubContext<NotificationHub> notificationHub;
        private readonly string myConnectionString;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config, IHubContext<NotificationHub> notificationHub)
        {
            _logger = logger;
            this.notificationHub = notificationHub;
            myConnectionString = config.GetConnectionString("db");
        }


        [HttpGet]
        [Route("/pingdb")]
        public IEnumerable<string> PingDb()
        {
            using var conn = new SqlConnection(myConnectionString);
            conn.Open();
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

        [HttpPost]
        [Route("/TestPost")]
        public Task TestPostAsync()
        {
            Console.WriteLine("test post");
            return Task.CompletedTask;
        }

        [HttpGet]
        [Route("/TestNotification")]
        public async Task<IActionResult> TestNotification()
        {
            await notificationHub.Clients.All.SendAsync("RecieveMessage", "Hola Mario");
            return Ok();
        }
    }


}