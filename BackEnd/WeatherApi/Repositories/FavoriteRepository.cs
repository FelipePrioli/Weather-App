using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.Models;

namespace WeatherApi.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _context;

        public FavoriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CidadeFavorita>> GetAllAsync(int usuarioId)
        {
            return await _context.CidadesFavoritas
                .Where(x => x.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task AddAsync(CidadeFavorita cidade)
        {
            _context.CidadesFavoritas.Add(cidade);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            var cidade = await _context.CidadesFavoritas.FindAsync(id);
            if (cidade == null) return;

            _context.CidadesFavoritas.Remove(cidade);
            await _context.SaveChangesAsync();
        }
    }
}
