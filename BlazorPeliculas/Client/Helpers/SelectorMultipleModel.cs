namespace BlazorPeliculas.Client.Helpers
{
    public class SelectorMultipleModel
    {
        public string Llave { get; set; }
        public string Valor { get; set; }
        public SelectorMultipleModel(string llave, string Valor)
        {
            this.Llave = llave;
            this.Valor = Valor;
        }
    }
}
