using Microsoft.AspNetCore.Mvc;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] string? city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("Cidade é obrigatória");

            try
            {
                var result = await _weatherService.GetCurrentWeather(city);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest("Cidade é obrigatória.");

            var forecast = await _weatherService.GetForecast(city);
            return Ok(forecast);
        }

    }

}
