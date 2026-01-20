using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using WeatherApi.Controllers;
using WeatherApi.DTOs.Users;
using WeatherApi.Models;
using WeatherApi.Services;
using WeatherApi.Data;

namespace WeatherApi.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            // Configura banco em memória
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new AppDbContext(options);
            _userService = new UserService(_dbContext); // serviço real usando InMemory
            _controller = new UserController(_userService);
        }

        [Fact]
        public async Task Create_DeveCriarUsuario_Corretamente()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Nome = "Felipe",
                Email = "felipe@example.com",
                Senha = "123456"
            };

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserResponseDto>(okResult.Value);

            Assert.Equal(createDto.Nome, response.Nome);
            Assert.Equal(createDto.Email, response.Email);

            // Limpeza: remove o usuário criado
            var usuarioCriado = await _dbContext.Usuarios.FindAsync(response.Id);
            if (usuarioCriado != null)
            {
                _dbContext.Usuarios.Remove(usuarioCriado);
                await _dbContext.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task Me_DeveRetornarUsuario_QuandoUsuarioExistir()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "Felipe",
                Email = "felipe@example.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456")
            };
            _dbContext.Usuarios.Add(usuario);
            await _dbContext.SaveChangesAsync();

            // Simula usuário logado
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await _controller.Me();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserResponseDto>(okResult.Value);

            Assert.Equal(usuario.Id, response.Id);
            Assert.Equal(usuario.Nome, response.Nome);
            Assert.Equal(usuario.Email, response.Email);

            // Limpeza
            _dbContext.Usuarios.Remove(usuario);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task Me_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
        {
            // Arrange
            var userIdInexistente = 999;

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userIdInexistente.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Act
            var result = await _controller.Me();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
