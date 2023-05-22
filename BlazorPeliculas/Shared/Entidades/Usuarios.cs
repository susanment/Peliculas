
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlazorPeliculas.Shared.Entidades
{
    public class Usuarios
    {
        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public string? EmailNormalizado { get; set; }
        public string? PasswordHash { get; set; }

    }
}
