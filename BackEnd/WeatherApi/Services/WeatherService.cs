using System.Text.Json;
using WeatherApi.DTOs;

namespace WeatherApi.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        

        public async Task<WeatherResponseDto> GetCurrentWeather(string city)
        {
            var apiKey = _configuration["OpenWeather:ApiKey"];

            var url =
                $"https://api.openweathermap.org/data/2.5/weather" +
                $"?q={city}&appid={apiKey}&units=metric&lang=pt_br";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(content);

            using var json = JsonDocument.Parse(content);
            var root = json.RootElement;

            return new WeatherResponseDto
            {
                City = root.GetProperty("name").GetString() ?? city,
                Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                TempMin = root.GetProperty("main").GetProperty("temp_min").GetDouble(),
                TempMax = root.GetProperty("main").GetProperty("temp_max").GetDouble(),
                Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
                Icon = root.GetProperty("weather")[0].GetProperty("icon").GetString() ?? ""
            };
        }

        public async Task<List<WeatherForecastDto>> GetForecast(string city)
        {
            var apiKey = _configuration["OpenWeather:ApiKey"];

            var url =
                $"https://api.openweathermap.org/data/2.5/forecast" +
                $"?q={city}&appid={apiKey}&units=metric&lang=pt_br";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(content);

            using var json = JsonDocument.Parse(content);
            var list = json.RootElement.GetProperty("list");

            // Agrupa por dia e pega apenas 5 dias
            var today = DateTime.UtcNow.Date;

            var result = list
                .EnumerateArray()
                .GroupBy(item =>
                    DateTime.Parse(item.GetProperty("dt_txt").GetString()!).Date
                )
                .Where(g => g.Key > today)
                .Take(5)
                .Select(day => new WeatherForecastDto
                {
                    Date = day.Key,
                    TempMin = day.Min(x => x.GetProperty("main").GetProperty("temp_min").GetDouble()),
                    TempMax = day.Max(x => x.GetProperty("main").GetProperty("temp_max").GetDouble()),
                    Description = day.First().GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
                    Icon = day.First().GetProperty("weather")[0].GetProperty("icon").GetString() ?? ""
                })
                .ToList();


            return result;
        }

    }
}
