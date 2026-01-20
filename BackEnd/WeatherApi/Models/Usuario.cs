using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models
{
    public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; }

    [EmailAddress]
    public string Email { get; set; }
    public string SenhaHash { get; set; }

    public ICollection<CidadeFavorita> CidadesFavoritas { get; set; } 
    = new List<CidadeFavorita>();
}
}

