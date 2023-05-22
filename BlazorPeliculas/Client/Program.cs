using BlazorPeliculas.Client;
using BlazorPeliculas.Client.Auth;
using BlazorPeliculas.Client.Repositorios;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
configureServices(builder.Services);
await builder.Build().RunAsync();

void configureServices(IServiceCollection services)
{
    //services.AddSingleton<ServicioSingleton>();
    //services.AddTransient<ServicioTransient>();
    //services.AddScoped<ServicioScope>();
    services.AddScoped<IRepositorio, Repositorio>();
    services.AddSweetAlert2();
    services.AddAuthorizationCore();
    
    services.AddScoped<ProveedorAuthenticacionJWT>();

    services.AddScoped<AuthenticationStateProvider, ProveedorAuthenticacionJWT>(proveedor =>
    proveedor.GetRequiredService<ProveedorAuthenticacionJWT>());

    services.AddScoped<ILoginService, ProveedorAuthenticacionJWT>(proveedor =>
    proveedor.GetRequiredService<ProveedorAuthenticacionJWT>());
    //services.AddScoped<RenovadorToken>();
}
