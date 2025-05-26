using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("Risco")]
    public class Risco
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "TEXT")]
        public string? DescricaoRisco { get; set; }

        [Column(TypeName = "VARCHAR(255)")]
        public string? AreaRisco { get; set; }

        [Column(TypeName = "INT")]
        public int? ProbabilidadeRisco { get; set; }

        [Column(TypeName = "INT")]
        public int? ImpactoRisco { get; set; }

        [Column(TypeName = "INT")]
        public int? NivelRisco { get; set; }



        // Referências externas
        public Guid IdAnaliseProjeto { get; set; }

        [ForeignKey("IdAnaliseProjeto")]
        public AnaliseProjeto? AnaliseProjeto { get; set; }

        public Guid IdTechSkill { get; set; }

        [ForeignKey("IdTechSkill")]
        public TechSkill? TechSkill { get; set; }
    }
}
