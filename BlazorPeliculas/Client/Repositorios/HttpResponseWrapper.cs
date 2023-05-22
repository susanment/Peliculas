using System.Net;

namespace BlazorPeliculas.Client.Repositorios
{
    public class HttpResponseWrapper<T>
    {
        public HttpResponseWrapper(T? response, bool error, HttpResponseMessage responseMessage) {
            Response = response;
            Error = error;
            HttpResponseMessage = responseMessage;
        }
        public bool Error { get; set; }
        public T? Response { get; set; }
        public HttpResponseMessage? HttpResponseMessage { get; set; }

        public async Task< string?> ObtenerMensajeError()
        {
            if (!Error)
            {
                return null;
            }
            
            var codigoEstatus= HttpResponseMessage.StatusCode;
            if (codigoEstatus== HttpStatusCode.NotFound)
            {
                return "Recurso no encontrado";
            }
            else if (codigoEstatus== HttpStatusCode.BadRequest)
            {
                return await HttpResponseMessage.Content.ReadAsStringAsync();
            }
            else if(codigoEstatus== HttpStatusCode.Unauthorized)
            {
                return "Tienes que loguearte a la aplicación";
            }
            else if (codigoEstatus== HttpStatusCode.Forbidden)
            {
                return "No tienes permisos";
            }
            else
            {
                return "Ha ocurridó un error inesperado";
            }
        }
    }
}
