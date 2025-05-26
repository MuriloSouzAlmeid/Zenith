using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("TechSkill")]
    public class TechSkill
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "VARCHAR(200)")]
        public string? Skill { get; set; }
    }
}
