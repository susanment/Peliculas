using BlazorPeliculas.Shared.Entidades;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace BlazorPeliculas.Server.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuarios> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<Usuarios> BuscarUsuarioPorId(int IdUsuario);
        Task<int> CrearUsuario(Usuarios usuario);
    }

    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        protected SqlConnection dbConnection()
        {
            return new SqlConnection(connectionString);
        }
        public RepositorioUsuarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionPeliculas");
        }

        public async Task<int> CrearUsuario(Usuarios usuario)
        {
            var db = dbConnection();
            var usuarioId = await db.QuerySingleAsync<int>(@"
                        INSERT INTO Usuarios (Email, EmailNormalizado, PasswordHash)
                        VALUES (@Email, @EmailNormalizado, @PasswordHash);
                        SELECT SCOPE_IDENTITY();
                        ", usuario);

            //await db.ExecuteAsync("CrearDatosUsuarioNuevo", new { usuarioId },
              //  commandType: System.Data.CommandType.StoredProcedure);

            return usuarioId;
        }

        public async Task<Usuarios> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            var db = dbConnection();
            var usuario = await db.QuerySingleOrDefaultAsync<Usuarios>(
                "SELECT * FROM Usuarios Where EmailNormalizado = @emailNormalizado",
                new { emailNormalizado });
            return usuario;
        }

        public async Task<Usuarios> BuscarUsuarioPorId(int IdUsuario)
        {
            var db = dbConnection();
            var usuario = await db.QuerySingleOrDefaultAsync<Usuarios>(
                "SELECT * FROM Usuarios Where IdUsuario = @IdUsuario",
                new { IdUsuario });
            return usuario;
        }
    }
}

