using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("Equipe")]
    public class Equipe
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "INT")]
        public int? QtdIntegrantes { get; set; }

        // referências externas

        public Guid IdProjeto { get; set; }

        [ForeignKey("IdProjeto")]
        public Projeto? Projeto { get; set; }

        List<Colaborador>? Colaboradores { get; set; }


        // Propriedade de navegação para a tabela intermediária
        public List<EquipeColaboradores>? EquipeColaboradores { get; set; }
    }
}
