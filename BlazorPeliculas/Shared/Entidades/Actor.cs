using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlazorPeliculas.Shared.Entidades
{
    public class Actor
    {
        public int IdActor { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Nombre { get; set; }
        public string? Biografia { get; set; }
        public string? Foto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        [NotMapped]
        public string? Personaje { get; set; }
        //public List<PeliculaActor> PeliculasActor { get; set; } = new List<PeliculaActor>();

        public override bool Equals(object obj)
        {
            if (obj is Actor a2)
            {
                return IdActor == a2.IdActor;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
