using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorPeliculas.Client.Auth
{
    public class ProveedorAutenticacionPrueba : AuthenticationStateProvider
    {
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonimo = new ClaimsIdentity();// un dato acerca del usuario(nombre, fecha, email)
            var usuarioyo=new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim("llave1","valor1"),
                    new Claim("edad","99"),
                    new Claim(ClaimTypes.Name,"Susana")
                },
                authenticationType: "prueba");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(usuarioyo)));
        }
    }
}
