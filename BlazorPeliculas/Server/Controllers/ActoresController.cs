using BlazorPeliculas.Shared.Entidades;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Server.Servicios;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BlazorPeliculas.Shared.DTO;


namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/actores")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly IRepositorioActores repositorioActores;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly string Contenedor = "personas";

        public ActoresController(IRepositorioActores repositorioActores,IAlmacenadorArchivos almacenadorArchivos, IMapper mapper)
        {
            this.repositorioActores = repositorioActores;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> Get([FromQuery]PaginacionDTO paginacion)
        {

            var TotalActores = await repositorioActores.ObtenerTotalActores();
            await HttpContext
                .InsertarParametrosPaginacionEnRespuesta(paginacion.CantidadRegistros, TotalActores);
            var ActoresList=await repositorioActores.ObtenerActores(paginacion.Pagina, paginacion.CantidadRegistros);
            return Ok(ActoresList);
        }
        
        [HttpGet("buscar/{textoBusqueda}")]//Variable de ruta
        public async Task<ActionResult<List<Actor>>> Get(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                return new List<Actor>();
            }
            var Actores = await repositorioActores.ObtenerActoresPorTextoBusqueda(textoBusqueda);
            return  Actores
                .Take(5)
                .ToList();
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Actor>> Get(int id)
        {
            var actor = await repositorioActores.ObtenerActor(id);
            if (actor is null)
            {
                return NotFound();
            }
            return actor;
        }
        [HttpPost]
        public async Task<ActionResult<int>> Post(Actor actor)
        {
            if (!string.IsNullOrEmpty(actor.Foto))
            {
                var fotoActor= Convert.FromBase64String(actor.Foto);
                actor.Foto = await almacenadorArchivos.GuardarArchivo(fotoActor, ".jpg", Contenedor);
            }
            await repositorioActores.CrearActor(actor);
            return actor.IdActor;
        }
        [HttpPut]
        public async Task<ActionResult> Put(Actor actor)
        {
            var actorDb = await repositorioActores.ObtenerActor(actor.IdActor);
            if (actorDb is null)
            { 
                return NotFound(); 
            }
            //este no va ?
            actorDb= mapper.Map(actor,actorDb);
            if (!string.IsNullOrWhiteSpace(actor.Foto))
            {
                //si se edita la foto se guarda
                var fotoActor = Convert.FromBase64String(actor.Foto);
                actorDb.Foto = await almacenadorArchivos.EditarArchivo(fotoActor, ".jpg", Contenedor, actorDb.Foto);
            }
            await repositorioActores.UpdateActor(actorDb);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor= repositorioActores.ObtenerActor(id);
            var ac = actor.Result;
            if (actor is null || ac is null)
            {
                return NotFound();
            }
            await almacenadorArchivos.EliminarArchivo(ac.Foto, Contenedor);
            var filasAfectadas = await repositorioActores.DeleteActor(id);
            if (filasAfectadas == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
