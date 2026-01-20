using WeatherApi.Models;

namespace WeatherApi.Repositories
{
    public interface IFavoriteRepository
    {
        Task<List<CidadeFavorita>> GetAllAsync(int usuarioId);
        Task AddAsync(CidadeFavorita cidade);
        Task RemoveAsync(int id);
    }
}
