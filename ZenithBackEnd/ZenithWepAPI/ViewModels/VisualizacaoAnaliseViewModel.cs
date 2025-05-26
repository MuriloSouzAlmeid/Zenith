using ZenithWepAPI.Domains;

namespace ZenithWepAPI.ViewModels
{
    public class VisualizacaoAnaliseViewModel
    {
        public Guid Id { get; set; }
        public string? Descricao { get; set; }
        public float[]? ComposicaoEquipeIdeal { get; set; }
        public List<VisualizarRiscoViewModel>? Riscos { get; set; }
        public VisualizarProjetoViewModel? Projeto { get; set; }
    }
}
