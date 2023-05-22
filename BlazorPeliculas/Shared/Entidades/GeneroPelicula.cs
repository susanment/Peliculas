namespace BlazorPeliculas.Shared.Entidades
{
    public class GeneroPelicula
    {
        public int IdPelicula { get; set; }
        public int IdGenero { get; set; }
        public Genero Genero { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
