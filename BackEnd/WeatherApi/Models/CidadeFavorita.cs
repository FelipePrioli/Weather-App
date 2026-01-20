namespace WeatherApi.Models
{
    public class CidadeFavorita
    {
        public int Id { get; set; }

        public string Nome { get; set; } = null!;

       
        public string NomeNormalizado { get; set; } = null!;

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
