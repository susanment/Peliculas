
namespace BlazorPeliculas.Server.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnRespuesta(
            this HttpContext context
            ,int CantidadRegistrosAMostrar , int TotalRegistros
            )
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            double conteo = TotalRegistros;
            double totalPaginas = Math.Ceiling( conteo /CantidadRegistrosAMostrar);
            context.Response.Headers.Add("conteno", conteo.ToString());
            context.Response.Headers.Add("totalPaginas", totalPaginas.ToString());
        }
    }
}
