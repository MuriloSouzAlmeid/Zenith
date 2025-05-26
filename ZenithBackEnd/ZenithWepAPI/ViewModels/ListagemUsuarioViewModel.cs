namespace ZenithWepAPI.ViewModels
{
    public class ListagemUsuarioViewModel
    {
        public Guid IdUsuario { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Cargo { get; set; }
        public string? Perfil { get; set; }
        public string? Senioridade { get; set; }
        public string? Foto { get; set; }
        public Guid? IdColaborador { get; set; }
        public List<string>? TechSkills { get; set; }
    }
}
