using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("ColaboradorTechSkills")]
    public class ColaboradorTechSkills
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // referências externas

        public Guid IdColaborador { get; set; }

        [ForeignKey("IdColaborador")]
        public Colaborador? Colaborador { get; set; }


        public Guid IdTechSkill { get; set; }

        [ForeignKey("IdTechSkill")]
        public TechSkill? TechSkill { get; set; }
    }
}
