using BlazorPeliculas.Shared.Entidades;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BlazorPeliculas.Server.Servicios
{
    public interface IRepositorioGeneros
    {
        Task CrearGenero(Genero genero);
        Task<int> DeleteGenero(int IdGenero);
        Task<Genero> ObtenerGenero(int IdGenero);
        Task<IEnumerable<Genero>> ObtenerGeneros();
        Task<IEnumerable<Genero>> ObtenerGenerosDePelicula(int IdPelicula);
        Task UpdateGenero(Genero genero);
    }
    public class RepositorioGeneros : IRepositorioGeneros
    {
        private readonly string connectionString;
        public RepositorioGeneros(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionPeliculas");
        }

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(connectionString);
        }

        public async Task CrearGenero(Genero genero)
        {
            var db = dbConnection();
            var IdGenero = await db.QuerySingleAsync<int>(@"INSERT INTO Generos(NombreGenero)
                                                                    VALUES( @NombreGenero);

                                                        SELECT SCOPE_IDENTITY();", genero);
            genero.IdGenero = IdGenero;
        }

        public async Task<IEnumerable<Genero>> ObtenerGeneros()
        {
            var db = dbConnection();
            var Generos = await db.QueryAsync<Genero>(@"
                                            select * from generos
                                    ");

            return Generos.ToList();
        }

        public async Task<IEnumerable<Genero>> ObtenerGenerosDePelicula(int IdPelicula)
        {
            var db = dbConnection();
            var Generos = await db.QueryAsync<Genero>(@"
                                            select G.*
                                            from GenerosPeliculas gp
                                            JOIN Generos G ON G.IdGenero=gp.IdGenero
                                            WHERE IdPelicula=@IdPelicula
                                    "
                                    ,param: new { IdPelicula}
                                    );

            return Generos.ToList();
        }
        public async Task<Genero> ObtenerGenero(int IdGenero)
        {
            var db = dbConnection();
            var Genero = await db.QueryFirstOrDefaultAsync<Genero>(@"
                                            select * from generos where IdGenero= @IdGenero
                                    "
                                    , param: new { IdGenero }
                                );

            return Genero;
        }

        public async Task UpdateGenero(Genero genero)
        {
            var db = dbConnection();
            await db.QueryAsync(@"
                        update Generos set NombreGenero=@NombreGenero where IdGenero=@IdGenero
                "
                ,genero
                );
        }
        public async Task<int> DeleteGenero(int IdGenero)
        {
            var db = dbConnection();
            var NumeroFilasAfectadas= await db.ExecuteAsync("DELETE FROM Generos where IdGenero=@IdGenero"
                , param: new { IdGenero}
                );
            return NumeroFilasAfectadas;
        }
    }
}
