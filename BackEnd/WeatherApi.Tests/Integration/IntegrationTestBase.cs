using Microsoft.AspNetCore.Mvc.Testing;

namespace WeatherApi.Tests.Base
{
    public abstract class IntegrationTestBase 
        : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient Client;

        protected IntegrationTestBase(
            WebApplicationFactory<Program> factory)
        {
            Client = factory.CreateClient();
        }
    }
}
