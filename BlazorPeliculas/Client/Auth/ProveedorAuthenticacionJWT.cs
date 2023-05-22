using BlazorPeliculas.Client.Helpers;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BlazorPeliculas.Client.Auth
{
    public class ProveedorAuthenticacionJWT : AuthenticationStateProvider,ILoginService
    {
        private readonly IJSRuntime jSRuntime;
        private readonly HttpClient httpClient;

        public ProveedorAuthenticacionJWT(IJSRuntime jSRuntime, HttpClient httpClient)
        {
            this.jSRuntime = jSRuntime;
            this.httpClient = httpClient;
        }
        public static readonly string TOKENKEY = "TOKENKEY";
        public static readonly string EXPIRATIONTOKENKEY = "EXPIRATIONTOKENKEY";
        private AuthenticationState Anonimo =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await jSRuntime.ObtenerDeLocalStorage(TOKENKEY);
            if (token is null) {
                return Anonimo;
            }
            return ConstruirAuthenticationState(token.ToString());
        }

        private AuthenticationState ConstruirAuthenticationState(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("bearer", token);
            var claims= ParsearClaimsDelJWT(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,"jwt")));
        }

        private IEnumerable<Claim> ParsearClaimsDelJWT(string token)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenDeserializado = jwtSecurityTokenHandler.ReadJwtToken(token);

            return tokenDeserializado.Claims;
        }

        public async Task Login(string token)
        {
            await jSRuntime.GuardarEnLocalStorage(TOKENKEY,token);
            var authState = ConstruirAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await jSRuntime.RemoverDeLocalStorage(TOKENKEY);
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }
    }
}
