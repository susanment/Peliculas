using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.DTO
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;//numero de pagina
        public int CantidadRegistros { get; set; } = 10;//cantidad a mostrar por pagina
    }
}
