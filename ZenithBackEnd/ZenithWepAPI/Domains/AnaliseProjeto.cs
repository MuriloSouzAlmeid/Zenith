using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("AnaliseProjeto")]
    public class AnaliseProjeto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "TEXT")]
        public string? DescricaoGeral { get; set; }

        [Column(TypeName = "FLOAT")]
        public float GestoresIdeais { get; set; }

        [Column(TypeName = "FLOAT")]
        public float SenioresIdeais { get; set; }

        [Column(TypeName = "FLOAT")]
        public float PlenosIdeais { get; set; }

        [Column(TypeName = "FLOAT")]
        public float JuniorsIdeais { get; set; }

        // referências externas

        public Guid IdProjeto { get; set; }

        [ForeignKey("IdProjeto")]
        public Projeto? Projeto { get; set; }

        public List<Risco>? Riscos { get; set; }
    }
}
