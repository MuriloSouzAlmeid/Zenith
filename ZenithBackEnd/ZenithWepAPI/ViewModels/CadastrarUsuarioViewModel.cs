using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ZenithWepAPI.Domains;

namespace ZenithWepAPI.ViewModels
{
    public class CadastrarUsuarioViewModel
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public int? NivelSenioridade { get; set; }
        public string? CargoUsuario { get; set; }
        public string[]? TechSkillsList { get; set; }
    }
}
