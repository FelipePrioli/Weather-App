using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Data;
using WeatherApi.Models;
using WeatherApi.DTOs.Auth;
using WeatherApi.DTOs.Favorite;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherApi.Tests.Controllers
{
    public class FavoritesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly IServiceScopeFactory _scopeFactory;

        public FavoritesControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        }

        /// <summary>
        /// Cria um usuário de teste no banco em memória e realiza login para obter token JWT
        /// </summary>
        private async Task AuthenticateAsync(string? email = null, string? senha = null)
        {
            email ??= $"{Guid.NewGuid()}@teste.com";
            senha ??= "123456";

            // Cria usuário no InMemoryDatabase
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

               
                var usuario = new Usuario
                {
                    Nome = "Teste",
                    Email = email,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha)
                };

                context.Usuarios.Add(usuario);
                await context.SaveChangesAsync();
            }

            // Login via endpoint
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                email,
                senha
            });

            loginResponse.EnsureSuccessStatusCode();

            var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            // Configura o HttpClient para usar o token
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", login!.Token);
        }

        [Fact(DisplayName = "POST /favorites deve salvar cidade favorita")]
        public async Task Post_Deve_Salvar_Cidade_Favorita()
        {
            await AuthenticateAsync();

            var response = await _client.PostAsJsonAsync(
                "/api/favorites",
                new FavoriteCreateDto { Nome = "São Paulo" }
            );

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var favorito = await response.Content.ReadFromJsonAsync<FavoriteResponseDto>();
            favorito!.Nome.Should().Be("São Paulo");
        }

        [Fact(DisplayName = "GET /favorites deve listar favoritos do usuário")]
        public async Task Get_Deve_Listar_Favoritos_Do_Usuario()
        {
            await AuthenticateAsync();

            await _client.PostAsJsonAsync(
                "/api/favorites",
                new FavoriteCreateDto { Nome = "Curitiba" }
            );

            var response = await _client.GetAsync("/api/favorites");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var favoritos = await response.Content
                .ReadFromJsonAsync<List<FavoriteResponseDto>>();

            favoritos.Should().ContainSingle();
            favoritos![0].Nome.Should().Be("Curitiba");
        }

        [Fact(DisplayName = "DELETE /favorites/{id} deve remover cidade favorita")]
        public async Task Delete_Deve_Remover_Cidade_Favorita()
        {
            await AuthenticateAsync();

            var post = await _client.PostAsJsonAsync(
                "/api/favorites",
                new FavoriteCreateDto { Nome = "Rio de Janeiro" }
            );

            var favorito = await post.Content.ReadFromJsonAsync<FavoriteResponseDto>();

            var response = await _client.DeleteAsync($"/api/favorites/{favorito!.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var get = await _client.GetAsync("/api/favorites");
            var list = await get.Content.ReadFromJsonAsync<List<FavoriteResponseDto>>();
            list.Should().BeEmpty();
        }
    }
}
