using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.Models;
using BCrypt.Net;

namespace WeatherApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        

        public async Task<Usuario> CreateAsync(
            string nome,
            string email,
            string senha)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha é obrigatória");

            var emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == email);

            if (emailExiste)
                throw new ArgumentException("Email já cadastrado");

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                SenhaHash = senhaHash
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }


        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
    }
}
