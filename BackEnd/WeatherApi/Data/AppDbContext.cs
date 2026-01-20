using Microsoft.EntityFrameworkCore;
using WeatherApi.Models;

namespace WeatherApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<CidadeFavorita> CidadesFavoritas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CidadeFavorita>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Nome)
                      .IsRequired()
                      .HasMaxLength(150);
            });
        }
    }
}
