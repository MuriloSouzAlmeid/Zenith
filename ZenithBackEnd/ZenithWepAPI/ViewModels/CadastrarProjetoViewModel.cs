using ZenithWepAPI.Domains;

namespace ZenithWepAPI.ViewModels
{
    public class CadastrarProjetoViewModel
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public string? TipoProjeto { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }
        public string[]? Tecnologias { get; set; }
        public string[]? Funcionalidades { get; set; }
    }
}
