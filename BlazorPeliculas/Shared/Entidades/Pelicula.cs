using System.ComponentModel.DataAnnotations;

namespace BlazorPeliculas.Shared.Entidades
{
    public class Pelicula
    {

        public int IdPelicula { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public bool EnCartelera { get; set; }
        public string Trailer { get; set; }
        public DateTime? Lanzamiento { get; set; } 
        public string Poster { get; set; }

        //public List<GeneroPelicula> GenerosPeliculas { get; set; }= new List<GeneroPelicula>();
        public List<Genero> Generos { get; set; }= new List<Genero>();
        public List<PeliculaActor> Actores { get; set; }= new List<PeliculaActor>();
        //public List<PeliculaActor> PeliculasActor { get; set; }= new List<PeliculaActor>();
        public string TituloCortado { get { 
            if(string.IsNullOrEmpty(Titulo))
                {
                    return null;
                }
                if (Titulo.Length>50)
                {
                    return Titulo.Substring(0, 60) + "...";

                }
                else
                {
                    return Titulo;
                }
            } }
    }
}
