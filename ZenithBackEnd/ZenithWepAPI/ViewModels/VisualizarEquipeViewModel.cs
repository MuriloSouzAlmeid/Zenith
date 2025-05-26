namespace ZenithWepAPI.ViewModels
{
    public class VisualizarEquipeViewModel
    {
        public Guid Id { get; set; }
        public int? QtdIntegrantes { get; set; }
        public List<ListagemUsuarioViewModel>? Colaboradores { get; set; }
    }
}
