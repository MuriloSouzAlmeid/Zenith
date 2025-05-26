using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("TipoProjeto")]
    public class TipoProjeto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "VARCHAR(255)")]
        public string? Tipo { get; set; }
    }
}
