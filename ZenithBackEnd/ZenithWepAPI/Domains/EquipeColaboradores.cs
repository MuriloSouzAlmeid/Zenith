using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("EquipeColaboradores")]
    public class EquipeColaboradores
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // referências externas

        public Guid IdEquipe { get; set; }

        [ForeignKey("IdEquipe")]
        public Equipe? Equipe { get; set; }


        public Guid IdColaborador { get; set; }

        [ForeignKey("IdColaborador")]
        public Colaborador? Colaborador { get; set; }
    }
}
