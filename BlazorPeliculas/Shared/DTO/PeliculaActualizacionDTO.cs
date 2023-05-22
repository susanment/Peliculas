﻿using BlazorPeliculas.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.DTO
{
    public class PeliculaActualizacionDTO
    {
        public Pelicula? Pelicula { get; set; } = null;
        public List<Actor> actores { get; set; } = new List<Actor>();
        public List<Genero> GenerosSeleccionados { get; set; } = new List<Genero>();
        public List<Genero> GenerosNoSeleccionados { get; set; } = new List<Genero>();
    }
       
}
