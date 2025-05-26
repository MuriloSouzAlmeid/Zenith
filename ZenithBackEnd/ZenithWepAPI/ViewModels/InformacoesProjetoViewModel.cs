namespace ZenithWepAPI.ViewModels
{
    public class InformacoesProjetoViewModel
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }
        public string? TipoProjeto { get; set; }
        public string[]? ListaFuncionalidades { get; set; }
        public string[]? ListaTecnologias { get; set; }
    }
}
