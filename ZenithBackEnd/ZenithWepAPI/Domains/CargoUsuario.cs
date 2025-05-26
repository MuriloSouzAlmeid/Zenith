using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("CargoUsuario")]
    public class CargoUsuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "VARCHAR(120)")]
        public string? Cargo { get; set; }

        [Column(TypeName = "INT")]
        public int NivelCargo { get; set; }

        [Column(TypeName = "VARCHAR(255)")]
        public string? Area { get; set; }
    }
}
