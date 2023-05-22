using BlazorPeliculas.Shared.Entidades;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;

namespace BlazorPeliculas.Server.Servicios
{
    public interface IRepositorioActores
    {
        Task CrearActor(Actor actor);
        Task<int> DeleteActor(int IdActor);
        Task<Actor> ObtenerActor(int IdActor);
        Task<IEnumerable<Actor>> ObtenerActores(int NumeroPagina, int PageSize);
        Task<IEnumerable<Actor>> ObtenerActoresPorTextoBusqueda(string TextoBusqueda);
        Task<int> ObtenerTotalActores();
        Task UpdateActor(Actor actor);
    }
    public class RepositorioActores : IRepositorioActores
    {
        private readonly string connectionString;

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(connectionString);
        }
        public RepositorioActores(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionPeliculas");
        }
        public async Task CrearActor(Actor actor)
        {
            var db = dbConnection();

            var IdActor = await db.QuerySingleAsync<int>(@"INSERT INTO Actores( Nombre, Biografia, Foto, FechaNacimiento)
                                                                VALUES(  @Nombre, @Biografia, @Foto, @FechaNacimiento);
                                                                SELECT SCOPE_IDENTITY();", actor);
            actor.IdActor = IdActor;
        }

        public async Task<IEnumerable<Actor>> ObtenerActores(int NumeroPagina, int PageSize)
        {
            var db = dbConnection();
            var actores = await db.QueryAsync<Actor>(@"
                                            select * 
                                            from Actores
                                            ORDER BY Nombre OFFSET @PageSize * (@NumeroPagina-1) ROWS 
                                            FETCH NEXT @PageSize ROWS ONLY
                                    "
                                    , new {PageSize,NumeroPagina}
                                    );

            return actores.ToList();
        }
        public async Task<int> ObtenerTotalActores()
        {
            var db = dbConnection();
            var actores = await db.ExecuteScalarAsync<int>(@"
                                            select count(IdActor) Total
                                            from Actores with(nolock);
                                    "
                                    );

            return actores;
        }

        public async Task<IEnumerable<Actor>> ObtenerActoresPorTextoBusqueda(string TextoBusqueda)
        {
            var db = dbConnection();
            var actores = await db.QueryAsync<Actor>(@"
                                            select * from Actores
                                            where lower(nombre) like '%' + @TextoBusqueda + '%';
                                    "
                                        , param: new { TextoBusqueda }  
                                        );

            return actores.ToList();
        }

        public async Task<Actor> ObtenerActor(int IdActor)
        {
            var db = dbConnection();
            var Genero = await db.QueryFirstOrDefaultAsync<Actor>(@"
                                            select *
                                            from Actores 
                                            where IdActor=@IdActor
                                    "
                                    , param: new { IdActor }
                                );

            return Genero;
        }

        public async Task UpdateActor(Actor actor)
        {
            var db = dbConnection();
            await db.QueryAsync(@"
                        update Actores set Biografia= @Biografia,FechaNacimiento=@FechaNacimiento,Foto=@Foto,Nombre=@Nombre where IdActor=@IdActor;
                "
                , actor
                );
        }

        public async Task<int> DeleteActor(int IdActor)
        {
            var db = dbConnection();
            var NumeroFilasAfectadas = await db.ExecuteAsync("DELETE FROM Actores where IdActor=@IdActor"
                , param: new { IdActor }
                );
            return NumeroFilasAfectadas;
        }
    }
}
