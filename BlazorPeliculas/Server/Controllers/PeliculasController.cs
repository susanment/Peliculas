using BlazorPeliculas.Shared.Entidades;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Server.Servicios;
using Microsoft.AspNetCore.Mvc;
using BlazorPeliculas.Shared.DTO;
using AutoMapper;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly IRepositorioPeliculas repositorioPeliculas;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IRepositorioGeneros repositorioGeneros;
        private readonly IMapper mapper;
        private readonly string Contenedor = "peliculas";

        public PeliculasController(IRepositorioPeliculas repositorioPeliculas
            , IAlmacenadorArchivos almacenadorArchivos
            , IRepositorioGeneros repositorioGeneros
            , IMapper mapper)
        {
            this.repositorioPeliculas = repositorioPeliculas;
            this.almacenadorArchivos = almacenadorArchivos;
            this.repositorioGeneros = repositorioGeneros;
            this.mapper = mapper;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var limite = 6;
            var PeliculasEnCartelera = await repositorioPeliculas.ObtenerPeliculasEnCaretelera();
            var EnCartelera = PeliculasEnCartelera.Where(x => x.EnCartelera).Take(limite)
                .OrderByDescending(x => x.Lanzamiento)
                .ToList();

            //var FechaActual = DateTime.Today;
            var ProximosEstrenos = await repositorioPeliculas.ObtenerPeliculasProximosEstrenos();

            var estrenos = ProximosEstrenos
                .OrderBy(x => x.Lanzamiento).Take(limite)
                .ToList();

            var resultado = new HomePageDTO
            {
                PeliculasEnCartelera = EnCartelera,
                ProximosEstrenos = estrenos
            };
            return resultado;
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaVisualizarDTO>> Get(int id)
        {

            var pelicula = await repositorioPeliculas.ObtenerPelicula(id);
            if (pelicula is null)
            {
                return NotFound();
            }
            //TODO: SISTEMA VOTACION
            var promedioVoto = 4;
            var VotoUsuario = 5;

            var modelo = new PeliculaVisualizarDTO();
            modelo.Pelicula = pelicula;
            modelo.Generos = pelicula.Generos.Select(x=>new Genero
            {
                IdGenero=x.IdGenero,
                NombreGenero=x.NombreGenero
            } ).ToList();
            var actoresPelicula= pelicula.Actores;
            modelo.Actores= actoresPelicula.Select(x=>new Actor
            {
                Nombre=x.Nombre,
                Foto=x.Foto,
                Personaje=x.Personaje,
                IdActor=    x.IdActor
            }
            ).ToList();
            modelo.PromedioVotos = promedioVoto;
            modelo.VotoUsuario = VotoUsuario;

            return modelo;
        }
        [HttpGet("filtrar")]
        public async Task<ActionResult<List<Pelicula>>> Get([FromQuery] ParametrosBusquedaPeliculasDTO modelo)
        {
            //en entity framework core se llama ejecucion diferida
            
            //Obtenemos total de registros
            var Total= await repositorioPeliculas.ObtenerCantidadPeliculasFiltradas(modelo);
            modelo.TotalRegistros = Total;
            //TODO: Implementar votacion
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(modelo.CantidadRegistros, modelo.TotalRegistros);

            //Obtenemos la lista
            var Peliculas = await repositorioPeliculas.ObtenerPeliculasFiltradas(modelo);

            return Ok(Peliculas);
        }
        [HttpGet("actualizar/{id}")]
        public async Task<ActionResult<PeliculaActualizacionDTO>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult)
            {
                return NotFound();
            }
            var peliculaVisualizarDTO = peliculaActionResult.Value;
            var generosSeleccionadosIds= peliculaVisualizarDTO.Generos.Select(x=>x.IdGenero).ToList();
            var generosNoSeleccionados = await repositorioGeneros.ObtenerGeneros();
            generosNoSeleccionados= generosNoSeleccionados.Where(x=> !generosSeleccionadosIds.Contains(x.IdGenero)).ToList();

            var modelo = new PeliculaActualizacionDTO();
            modelo.Pelicula = peliculaVisualizarDTO.Pelicula;
            modelo.GenerosNoSeleccionados = generosNoSeleccionados.ToList();
            modelo.GenerosSeleccionados = peliculaVisualizarDTO.Generos;
            modelo.actores = peliculaVisualizarDTO.Actores;
            return modelo;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Pelicula pelicula)
        {
            if (!string.IsNullOrEmpty(pelicula.Poster))
            {
                var posterActor = Convert.FromBase64String(pelicula.Poster);
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(posterActor, ".jpg", Contenedor);
            }
            EscribirOrdenActores(pelicula);
            await repositorioPeliculas.CrearPelicula(pelicula);
            return pelicula.IdPelicula;
        }

        private static void EscribirOrdenActores(Pelicula pelicula)
        {
            if (pelicula.Actores is not null)
            {
                for (int i = 0; i < pelicula.Actores.Count; i++)
                {
                    pelicula.Actores[i].Orden = i + 1;
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(Pelicula pelicula)
        {
            var peliculaDB = await repositorioPeliculas.ObtenerPelicula(pelicula.IdPelicula);
            if (peliculaDB is null)
            {
                return NotFound();
            }
            peliculaDB = mapper.Map(pelicula, peliculaDB);
            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var posterImagen = Convert.FromBase64String(pelicula.Poster);
                peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(posterImagen,".jpg", Contenedor, peliculaDB.Poster!);
            }
            EscribirOrdenActores(peliculaDB);
            await repositorioPeliculas.UpdatePelicula(peliculaDB);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pelicula = repositorioPeliculas.ObtenerPelicula(id);
            var pel = pelicula.Result;
            if (pelicula is null || pel is null)
            {
                return NotFound();
            }
            await almacenadorArchivos.EliminarArchivo(pel.Poster, Contenedor);
            var filasAfectadas = await repositorioPeliculas.DeletePelicula(id);
            if (filasAfectadas == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
