using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("Colaborador")]
    public class Colaborador
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Referências externas

        public Guid IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }
    }
}
