using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("Usuario")]
    [Index(nameof(Email), IsUnique = true)]
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "VARCHAR(200)")]
        public string? Nome { get; set; }

        [Column(TypeName = "VARCHAR(150)")]
        public string? Email { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Senha { get; set; }

        [Column(TypeName = "VARCHAR(4)")]
        public string? CodRecupSenha { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Foto { get; set; }

        [Column(TypeName = "INT")]
        public int? NivelSenioridade { get; set;}


        // Referências externas
        public Guid IdCargoUsuario { get; set; }

        [ForeignKey("IdCargoUsuario")]
        public CargoUsuario? CargoUsuario { get; set; }
    }
}
