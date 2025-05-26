using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ZenithWepAPI.ViewModels
{
    public class AtualizarUsuarioViewModel
    {
        public string? NovoNome { get; set; }
        public string? NovoEmail { get; set; }
        public int? NovoNivelSenioridade { get; set; }
        public string? NovoCargo { get; set; }

        public string[]? NovasTechSkillsList { get; set; }
    }
}
