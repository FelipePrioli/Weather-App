using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.DTOs.Favorite;
using WeatherApi.Models;
using WeatherApi.Helpers;

namespace WeatherApi.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly AppDbContext _context;

        public FavoriteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FavoriteResponseDto>> GetFavorites(int usuarioId)
        {
            return await _context.CidadesFavoritas
                .Where(c => c.UsuarioId == usuarioId)
                .Select(c => new FavoriteResponseDto
                {
                    Id = c.Id,
                    Nome = c.Nome
                })
                .ToListAsync();
        }

        public async Task<FavoriteResponseDto?> AddFavorite(
    FavoriteCreateDto dto,
    int usuarioId
    )
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == usuarioId);

        if (!usuarioExiste)
            return null;

        var normalizedCity = CityNormalizer.Normalize(dto.Nome);

        var alreadyExists = await _context.CidadesFavoritas
            .AnyAsync(c =>
                c.UsuarioId == usuarioId &&
                c.NomeNormalizado == normalizedCity
            );

        if (alreadyExists)
            return null;

        var entity = new CidadeFavorita
        {
            Nome = dto.Nome.Trim(),
            NomeNormalizado = normalizedCity,
            UsuarioId = usuarioId
        };

        _context.CidadesFavoritas.Add(entity);
        await _context.SaveChangesAsync();

        return new FavoriteResponseDto
        {
            Id = entity.Id,
            Nome = entity.Nome
        };
    }



        public async Task RemoveFavorite(int id, int usuarioId)
        {
            var cidade = await _context.CidadesFavoritas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (cidade == null)
                throw new Exception("Cidade não encontrada");

            _context.CidadesFavoritas.Remove(cidade);
            await _context.SaveChangesAsync();
        }
    }
}
