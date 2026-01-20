using WeatherApi.Models;

namespace WeatherApi.Repositories
{
    public interface IUserRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task AddAsync(Usuario usuario);
        Task<bool> ExistsAsync(int id);
    }
}
