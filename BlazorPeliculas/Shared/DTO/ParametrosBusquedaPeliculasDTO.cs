using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.DTO
{
    public class ParametrosBusquedaPeliculasDTO
    {
        public int Pagina { get; set; } = 1;
        public int CantidadRegistros { get; set; } = 10;
        public int TotalRegistros { get; set; }
        public PaginacionDTO PaginacionDTO
        {
            get
            {
                return new PaginacionDTO() { Pagina = Pagina, CantidadRegistros = CantidadRegistros };
            }
        }
        public string? Titulo { get; set; } = "";
        public int IdGenero { get; set; }
        public bool EnCartelera { get; set; }    
        public bool Estrenos { get; set; }    
        public bool MasVotadas { get; set; }    
    }
}
