using WeatherApi.Models;

namespace WeatherApi.Services
{
    public interface IUserService
    {
        Task<Usuario> CreateAsync(string nome, string email, string senha);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByIdAsync(int id);
    }
}
