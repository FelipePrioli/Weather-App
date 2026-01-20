namespace WeatherApi.DTOs
{
    public class WeatherResponseDto
    {
        public string City { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Humidity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
