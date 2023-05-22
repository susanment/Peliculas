using BlazorPeliculas.Client.Helpers;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPeliculas.Client.Pages
{
    public partial class Counter
    {
        //[Inject] ServicioSingleton singleton { get; set; }
        //[Inject] ServicioTransient transiente { get; set; }
        [Inject] IJSRuntime jS {  get; set; }
        //[CascadingParameter(Name = "Color")] protected string Color { get; set; }
        //[CascadingParameter(Name ="Size")] protected string Size { get; set; }
        //[CascadingParameter] protected AppState appState { get; set; }
        //IJSObjectReference modulo;
        private int currentCount = 0;

        //private static int CurrentCountStatic = 0;

        //[JSInvokable]
        public async Task IncrementCount()
        {
            
            //modulo = await jS.InvokeAsync<IJSObjectReference>("import","./js/Counter3.js");
            //await modulo.InvokeVoidAsync("mostrarAlerta");
            currentCount += 1;
            var arreglo = new double[] { 1,2,3,4,5,6};
            var max = arreglo.Maximum();
            var min= arreglo.Minimum();
            //CurrentCountStatic = currentCount;
            //singleton.Valor = currentCount;
            //transiente.Valor = currentCount;
            //scope.Valor = currentCount;
            //await jS.InvokeVoidAsync("PruebaPuntoNetStatic",
            //DotNetObjectReference.Create(this));
            await jS.InvokeVoidAsync("alert",$"El max es {max} y el min es {min}");
        }
        //private async Task IncrementCountJavaScript()
        //{

        //    await jS.InvokeVoidAsync("pruebaPuntoNetInstancia");
        //}
        
        //[JSInvokable]
        //public static Task<int> ObtenerCurrentCount()
        //{
        //    return Task.FromResult(CurrentCountStatic);
        //}

        //[JSInvokable]
        //public async Task Prueba()
        //{

        //    //await jS.InvokeVoidAsync("mostrarAlerta3", "Hola");
        //    await modulo.InvokeVoidAsync("mostrarAlerta2", "Hola");
        //}
    }
}
