using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("TecnologiasProjeto")]
    public class TecnologiasProjeto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // referências externas

        public Guid IdTecnologia { get; set; }

        [ForeignKey("IdTecnologia")]
        public Tecnologia? Tecnologia { get; set; }
 
        
        public Guid IdProjeto { get; set; }

        [ForeignKey("IdProjeto")]
        public Projeto? Projeto { get; set; }
    }
}
