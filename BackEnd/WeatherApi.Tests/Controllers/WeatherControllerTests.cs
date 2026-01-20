using System.Net;
using FluentAssertions;
using WeatherApi.Tests.Base;
using Xunit;

namespace WeatherApi.Tests.Controllers
{
    public class WeatherControllerTests : IntegrationTestBase
    {
        public WeatherControllerTests(
            Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> factory)
            : base(factory) { }

        [Fact]
        public async Task Deve_retornar_200_quando_cidade_for_valida()
        {
            // Arrange
            var city = "London";

            // Act
            var response = await Client.GetAsync(
                $"/api/weather?city={Uri.EscapeDataString(city)}"
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Deve_retornar_400_quando_cidade_nao_for_informada()
        {
            // Act
            var response = await Client.GetAsync("/api/weather");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest); 
        }

        [Fact]
        public async Task Deve_retornar_200_e_lista_de_previsoes_quando_cidade_for_valida()
        {
            // Arrange
            var city = "London";

            // Act
            var response = await Client.GetAsync(
                $"/api/weather/forecast?city={Uri.EscapeDataString(city)}"
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Forecast_deve_retornar_400_quando_cidade_nao_for_informada()
        {
            // Act
            var response = await Client.GetAsync("/api/weather/forecast");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Forecast_deve_retornar_no_maximo_5_dias_com_dados_validos()
        {
            // Arrange
            var city = "London";

            // Act
            var response = await Client.GetAsync(
                $"/api/weather/forecast?city={Uri.EscapeDataString(city)}"
            );

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            var forecast = System.Text.Json.JsonSerializer.Deserialize<
                List<WeatherApi.DTOs.WeatherForecastDto>
            >(content, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Assert
            forecast.Should().NotBeNull();
            forecast.Should().NotBeEmpty();
            forecast!.Count.Should().BeLessOrEqualTo(5);

            forecast.Should().OnlyContain(day =>
                day.Date != default &&
                day.TempMin <= day.TempMax &&
                !string.IsNullOrWhiteSpace(day.Description) &&
                !string.IsNullOrWhiteSpace(day.Icon)
            );
        }


    }
}
