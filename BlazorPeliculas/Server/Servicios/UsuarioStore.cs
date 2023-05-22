using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Identity;

namespace BlazorPeliculas.Server.Servicios
{
    public class UsuarioStore : IUserStore<Usuarios>, IUserEmailStore<Usuarios>,
        IUserPasswordStore<Usuarios>

    {
        private readonly IRepositorioUsuarios repositorioUsuarios;

        public UsuarioStore(IRepositorioUsuarios repositorioUsuarios)
        {
            this.repositorioUsuarios = repositorioUsuarios;
        }
        public async Task<IdentityResult> CreateAsync(Usuarios user, CancellationToken cancellationToken)
        {
            user.IdUsuario = await repositorioUsuarios.CrearUsuario(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Usuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //await repositorioUsuarios.EliminarUsuario(user);
            //return IdentityResult.Success;
        }

        public void Dispose()
        {

        }

        public async Task<Usuarios> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await repositorioUsuarios.BuscarUsuarioPorEmail(normalizedEmail);
        }

        public async Task<Usuarios> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            Usuarios usuarios = new Usuarios();
            var user = await repositorioUsuarios.BuscarUsuarioPorId(Convert.ToInt32(userId));

            return user;

        }

        public async Task<Usuarios> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await repositorioUsuarios.BuscarUsuarioPorEmail(normalizedUserName);
        }

        public Task<string> GetEmailAsync(Usuarios user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(Usuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(Usuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(Usuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(Usuarios user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(Usuarios user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.IdUsuario.ToString());
        }

        public Task<string> GetUserNameAsync(Usuarios user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(Usuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(Usuarios user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(Usuarios user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(Usuarios user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.EmailNormalizado = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(Usuarios user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(Usuarios user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(Usuarios user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> UpdateAsync(Usuarios user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //user.IdUsuario = await repositorioUsuarios.EditarUsuario(user);
            //return IdentityResult.Success;
        }
    }
}
