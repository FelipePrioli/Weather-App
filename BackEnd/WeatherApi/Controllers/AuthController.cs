using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherApi.DTOs.Auth;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IUserService userService,
            IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);

            if (user == null)
                return Unauthorized("Credenciais inválidas");

            // Aplica Trim() para evitar espaços invisíveis
            string senhaDigitada = request.Senha?.Trim() ?? "";
            string senhaHashBanco = user.SenhaHash?.Trim() ?? "";

            bool senhaValida = false;

            try
            {
                // Verifica a senha usando BCrypt
                senhaValida = BCrypt.Net.BCrypt.Verify(senhaDigitada, senhaHashBanco);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Retorna não autorizado se o hash do banco estiver inválido
                return Unauthorized("Credenciais inválidas");
            }

            if (!senhaValida)
                return Unauthorized("Credenciais inválidas");

            var token = GerarToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token
            });
        }

        private string GerarToken(Usuario user)
        {
            var jwt = _configuration.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nome)
            };

            var key = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(jwt["Key"]!)
            );

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(jwt["ExpiresInMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
