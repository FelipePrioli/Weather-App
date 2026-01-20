using WeatherApi.DTOs.Favorite;

namespace WeatherApi.Services
{
    public interface IFavoriteService
    {
        Task<List<FavoriteResponseDto>> GetFavorites(int usuarioId);
        Task<FavoriteResponseDto> AddFavorite(FavoriteCreateDto dto, int usuarioId);
        Task RemoveFavorite(int id, int usuarioId);
    }
}
