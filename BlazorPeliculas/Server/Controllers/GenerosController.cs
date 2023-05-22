using BlazorPeliculas.Shared.Entidades;
using BlazorPeliculas.Server.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly IRepositorioGeneros repositorioGeneros;

        public GenerosController(IRepositorioGeneros repositorioGeneros)
        {
            this.repositorioGeneros = repositorioGeneros;
        }
        [HttpGet]   
        public async Task<ActionResult<IEnumerable<Genero>>> Get() {
            var Generos= await repositorioGeneros.ObtenerGeneros();
            return Ok(Generos);
            
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<Genero>>> GetGenerosPelicula(int id)
        {
            var Generos = await repositorioGeneros.ObtenerGenerosDePelicula(id);
            return Ok(Generos);

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genero>> Get(int id)
        {
            var genero= await repositorioGeneros.ObtenerGenero(id);
            if (genero is null)
            {
                return NotFound();
            }
            return genero;
        }
        [HttpPost]
        public async Task<ActionResult<int>> Post(Genero genero)
        {
            await repositorioGeneros.CrearGenero(genero);
            return genero.IdGenero;
        }
        [HttpPut]
        public async Task<ActionResult> Put(Genero genero)
        {
            await repositorioGeneros.UpdateGenero(genero);
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            
            var filasAfectadas = await repositorioGeneros.DeleteGenero(id);
            if (filasAfectadas==0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
