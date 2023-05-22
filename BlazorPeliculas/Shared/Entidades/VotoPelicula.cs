namespace BlazorPeliculas.Shared.Entidades
{
    public class VotoPelicula
    {
        public int IdVotoPelicula { get; set; }
        public int Voto { get; set; }
        public DateTime FechaVoto { get; set; }
        public int IdPelicula { get; set; }
        public Pelicula? Pelicula { get; set; }
    }
}
