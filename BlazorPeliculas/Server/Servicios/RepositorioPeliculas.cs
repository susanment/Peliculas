using Azure;
using BlazorPeliculas.Shared.DTO;
using BlazorPeliculas.Shared.Entidades;
using Dapper;
using Microsoft.Extensions.Hosting;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices.Marshalling;
using static Dapper.SqlMapper;

namespace BlazorPeliculas.Server.Servicios
{
    public interface IRepositorioPeliculas
    {
        Task CrearPelicula(Pelicula pelicula);
        Task<int> DeletePelicula(int IdPelicula);
        Task<int> ObtenerCantidadPeliculasFiltradas(ParametrosBusquedaPeliculasDTO modelo);
        Task<Pelicula> ObtenerPelicula(int IdPelicula);
        Task<IEnumerable<Pelicula>> ObtenerPeliculas();
        Task<IEnumerable<Pelicula>> ObtenerPeliculasEnCaretelera();
        Task<IEnumerable<Pelicula>> ObtenerPeliculasFiltradas(ParametrosBusquedaPeliculasDTO modelo);
        Task<IEnumerable<Pelicula>> ObtenerPeliculasProximosEstrenos();
        Task UpdatePelicula(Pelicula pelicula);
    }
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly string connectionString;
        protected SqlConnection dbConnection()
        {
            return new SqlConnection(connectionString);
        }
        public RepositorioPeliculas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionPeliculas");
        }
        public async Task CrearPelicula(Pelicula pelicula)
        {
            var db = dbConnection();
            db.Open();
            try
            {
                using (var transaction = db.BeginTransaction())
                {
                    var IdPelicula = await db.QuerySingleAsync<int>(@"INSERT INTO Peliculas(Titulo, Resumen, EnCartelera, Trailer, Lanzamiento, Poster)
                                                                    VALUES(@Titulo, @Resumen, @EnCartelera, @Trailer, @Lanzamiento, @Poster);
                                                                    SELECT SCOPE_IDENTITY();", pelicula, transaction: transaction);
                    pelicula.IdPelicula = IdPelicula;

                    foreach (var item in pelicula.Actores)
                    {

                        await db.QueryAsync(@"
                            INSERT INTO PeliculasActores(IdActor,IdPelicula,Orden,Personaje)
                            SELECT @IdActor,@IdPelicula,@Orden,@Personaje
                        ",
                            param: new { item.IdActor, IdPelicula, item.Orden, item.Personaje }
                            , transaction: transaction
                            );

                    }
                    foreach (var item in pelicula.Generos)
                    {
                        await db.QueryAsync(@"INSERT INTO GenerosPeliculas(IdGenero,IdPelicula)
                                            SELECT @IdGenero,@IdPelicula",
                                                param: new { item.IdGenero, IdPelicula }
                                                , transaction: transaction
                                                );

                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<IEnumerable<Pelicula>> ObtenerPeliculasFiltradas(ParametrosBusquedaPeliculasDTO modelo)
        {

            var query = @"
                                 SELECT distinct p.*
                                FROM Peliculas P 
                                INNER JOIN GenerosPeliculas GP ON GP.IdPelicula=P.IdPelicula
                                INNER JOIN Generos G ON G.IdGenero=GP.IdGenero
                                WHERE 1=1
                                ";

            var dynamicParameters = new DynamicParameters();
            dynamicParameters = armarParametros(modelo, ref query);


            dynamicParameters.Add("NumeroPagina", modelo.Pagina);
            dynamicParameters.Add("PageSize", modelo.CantidadRegistros);
            query += @" ORDER BY Titulo OFFSET @PageSize * (@NumeroPagina-1) ROWS 
                                            FETCH NEXT @PageSize ROWS ONLY";

            var db = dbConnection();


            var peliculas = await db.QueryAsync<Pelicula>(query, dynamicParameters);

            return peliculas.ToList();
        }

        private DynamicParameters armarParametros(ParametrosBusquedaPeliculasDTO modelo, ref string query)
        {
            var dynamicParameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(modelo.Titulo))
            {
                query += @"AND P.Titulo like @Titulo";
                dynamicParameters.Add("Titulo", $"%{modelo.Titulo}%");
            }
            if (modelo.EnCartelera)
            {
                query += @" AND P.EnCartelera = @EnCartelera";
                dynamicParameters.Add("EnCartelera", modelo.EnCartelera);
            }
            if (modelo.Estrenos)
            {
                query += @" AND Lanzamiento >= @Lanzamiento";
                var hoy = DateTime.Today;
                dynamicParameters.Add("Lanzamiento", hoy);
            }
            if (modelo.IdGenero != 0)
            {
                query += @" AND G.IdGenero = @IdGenero";
                dynamicParameters.Add("IdGenero", modelo.IdGenero);
            }
            return dynamicParameters;
        }

        public async Task<int> ObtenerCantidadPeliculasFiltradas(ParametrosBusquedaPeliculasDTO modelo)
        {

            var query = @"
                                 SELECT count(distinct p.idpelicula) Total
                                FROM Peliculas P with (nolock)
                                INNER JOIN GenerosPeliculas GP with (nolock) ON GP.IdPelicula=P.IdPelicula
                                INNER JOIN Generos G with (nolock) ON G.IdGenero=GP.IdGenero
                                WHERE 1=1
                                ";

            var dynamicParameters = new DynamicParameters();
            dynamicParameters= armarParametros(modelo, ref query);

            var db = dbConnection();


            var peliculas = await db.ExecuteScalarAsync<int>(query, dynamicParameters);

            return peliculas;
        }
        public async Task<IEnumerable<Pelicula>> ObtenerPeliculas()
        {
            var db = dbConnection();
            var Peliculas = await db.QueryAsync<Pelicula>(@"
                                            select * 
                                            from Peliculas
                                    ");
            foreach (var item in Peliculas)
            {
                item.Actores = new List<PeliculaActor>();
                item.Generos = new List<Genero>();
            }

            return Peliculas.ToList();
        }

        public async Task<IEnumerable<Pelicula>> ObtenerPeliculasEnCaretelera()
        {
            var db = dbConnection();
            var Peliculas = await db.QueryAsync<Pelicula>(@"
                                            select * from Peliculas 
                                            where EnCartelera=1
                                            ORDER BY Lanzamiento desc
                                    ");

            return Peliculas.ToList();
        }
        public async Task<IEnumerable<Pelicula>> ObtenerPeliculasProximosEstrenos()
        {
            var db = dbConnection();
            var Peliculas = await db.QueryAsync<Pelicula>(@"
                                            select * from Peliculas 
                                            where Lanzamiento > GETDATE()
                                            order by Lanzamiento 
                                    ");

            return Peliculas.ToList();
        }

        public async Task<Pelicula> ObtenerPelicula(int IdPelicula)
        {
            try
            {

                var db = dbConnection();

                await db.OpenAsync();
                var result = await db.QueryMultipleAsync(@"
                                        select p.IdPelicula,p.Titulo,p.Resumen,p.EnCartelera,p.Trailer,p.Lanzamiento,p.Poster
                                        from Peliculas p
                                        where p.IdPelicula = @IdPelicula

                                        select g.IdGenero IdGenero,g.NombreGenero
                                        from GenerosPeliculas gp 
                                        inner join  Generos g on g.IdGenero=gp.IdGenero  
                                        where gp.IdPelicula = @IdPelicula

                                        select a.IdActor ,a.Nombre,a.Biografia,a.Foto,a.FechaNacimiento,pa.Personaje,pa.Orden
                                        from PeliculasActores pa
                                        inner join Actores a on a.IdActor=pa.IdActor 
                                        where pa.IdPelicula = @IdPelicula
                "
                , param: new { IdPelicula });
                var list = await result.ReadAsync<Pelicula>();
                var pelicula = list.FirstOrDefault();
                var generos = await result.ReadAsync<Genero>();
                var actores = await result.ReadAsync<PeliculaActor>();

                pelicula.Generos = generos.ToList();
                pelicula.Actores = actores.ToList();

                //     var Peliculas = await db.QueryAsync<Pelicula, Genero, PeliculaActor, Pelicula>(@"
                //                                 select p.IdPelicula,p.Titulo,p.Resumen,p.EnCartelera,p.Trailer,p.Lanzamiento,p.Poster,g.IdGenero IdGenero,g.NombreGenero, a.IdActor ,a.Nombre,a.Biografia,a.Foto,a.FechaNacimiento,pa.Personaje,pa.Orden
                //                                 from Peliculas p
                //                                 inner join GenerosPeliculas gp on gp.IdPelicula=p.IdPelicula
                //                                 inner join  Generos g on g.IdGenero=gp.IdGenero  
                //Inner join PeliculasActores pa on pa.IdPelicula=p.IdPelicula
                //inner join Actores a on a.IdActor=pa.IdActor 
                //                                 where p.IdPelicula = @IdPelicula
                //                         "
                //             , map: (peliculas, generos, peliculaActor) =>
                //             {
                //                 peliculas.Generos.Add(generos);
                //                 peliculas.Actores.Add(peliculaActor);
                //                 return peliculas;
                //             },
                //             splitOn: "IdGenero,IdActor",
                //      param: new { IdPelicula });

                //     var Actores = Peliculas.GroupBy(p => p.Actores).Select(g =>
                //     {
                //         var groupActores = g.First();
                //         groupActores.Actores = g.Select(p => p.Actores.Single()).ToList();
                //         return groupActores;
                //     });
                //     var Generos = Peliculas.GroupBy(p => p.Generos).Select(g =>
                //     {
                //         var groupGeneros = g.First();
                //         groupGeneros.Generos = g.Select(p => p.Generos.Single()).ToList();
                //         return groupGeneros;
                //     });
                //     
                //     pelicula = Peliculas.FirstOrDefault();
                //     foreach (var item in Generos)
                //     {
                //         foreach (var genero in item.Generos)
                //         {
                //             pelicula.Generos.Add(genero);
                //         }

                //     }
                //     foreach (var item in Actores)
                //     {
                //         foreach (var actor in item.Actores)
                //         {
                //             pelicula.Actores.Add(actor);
                //         }
                //     }
                return pelicula;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task UpdatePelicula(Pelicula pelicula)
        {
            var db = dbConnection();
            db.Open();
            try
            {
                using (var transaction = db.BeginTransaction())
                {
                    await db.QueryAsync(@"update Peliculas set EnCartelera=@EnCartelera,Poster=@Poster,Resumen=@Resumen,Titulo=@Titulo,Trailer=@Trailer where IdPelicula=@IdPelicula;
                                                                    delete from PeliculasActores where IdPelicula=@IdPelicula;
                                                                    delete from GenerosPeliculas where IdPelicula=@IdPelicula;

                            ", pelicula, transaction: transaction);


                    foreach (var item in pelicula.Actores)
                    {

                        await db.QueryAsync(@"
                            INSERT INTO PeliculasActores(IdActor,IdPelicula,Orden,Personaje)
                            SELECT @IdActor,@IdPelicula,@Orden,@Personaje
                        ",
                            param: new { item.IdActor, pelicula.IdPelicula, item.Orden, item.Personaje }
                            , transaction: transaction
                            );

                    }
                    foreach (var item in pelicula.Generos)
                    {
                        await db.QueryAsync(@"INSERT INTO GenerosPeliculas(IdGenero,IdPelicula)
                                            SELECT @IdGenero,@IdPelicula",
                                                param: new { item.IdGenero, pelicula.IdPelicula }
                                                , transaction: transaction
                                                );

                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<int> DeletePelicula(int IdPelicula)
        {
            var db = dbConnection();
            db.Open();
            try
            {
                using (var transaction = db.BeginTransaction())
                {
                    var NumeroFilasAfectadas = await db.ExecuteAsync(@"DELETE Peliculas where IdPelicula=@IdPelicula;
                                        delete from PeliculasActores where IdPelicula=@IdPelicula;
                                        delete from GenerosPeliculas where IdPelicula=@IdPelicula;

                            ", new { IdPelicula }, transaction: transaction);
                    transaction.Commit();
                    return NumeroFilasAfectadas;
                }

            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
