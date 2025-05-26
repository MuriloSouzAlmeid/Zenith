using ZenithWepAPI.Domains;

namespace ZenithWepAPI.ViewModels
{
    public class VisualizarProjetoViewModel
    {
        public Guid Id { get; set; }
        public ListagemUsuarioViewModel? Gerente { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public string? TipoProjeto { get; set; }
        public string? NivelProjeto { get; set; }
        public int? NivelAnalise { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }
        public List<Tecnologia>? Tecnologias { get; set; }
        public List<Funcionalidade>? Funcionalidades { get; set; }
        public VisualizarEquipeViewModel? Equipe { get; set; }
        public List<Risco>? Riscos { get; set; } 
    }
}
