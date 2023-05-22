using System.ComponentModel.DataAnnotations;

namespace BlazorPeliculas.Shared.Entidades
{
    public class Genero
    {
        public int IdGenero { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(50)]
        
        public string NombreGenero { get; set;}

        //public List<GeneroPelicula> GenerosPelicula { get; set; }= new List<GeneroPelicula>();
    }
}
