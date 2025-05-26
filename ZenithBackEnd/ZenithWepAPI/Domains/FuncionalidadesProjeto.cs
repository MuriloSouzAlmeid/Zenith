using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("FuncionalidadesProjeto")]
    public class FuncionalidadesProjeto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // referências externas

        public Guid IdFuncionalidade { get; set; }

        [ForeignKey("IdFuncionalidade")]
        public Funcionalidade? Funcionalidade { get; set; }


        public Guid IdProjeto { get; set; }

        [ForeignKey("IdProjeto")]
        public Projeto? Projeto { get; set; }
    }
}
