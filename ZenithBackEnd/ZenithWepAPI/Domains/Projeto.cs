using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenithWepAPI.Domains
{
    [Table("Projeto")]
    public class Projeto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column(TypeName = "VARCHAR(255)")]
        public string? Nome { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Descricao { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? DataInicio { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? DataFinal { get; set; }

        [Column(TypeName = "INT")]
        public int? NivelProjeto { get; set; }

        [Column(TypeName = "INT")]
        public int? NivelAnalise { get; set; }

        // referências externas

        public Guid IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }


        public Guid IdTipoProjeto { get; set; }

        [ForeignKey("IdTipoProjeto")]
        public TipoProjeto? TipoProjeto { get; set; }

        public AnaliseProjeto? Analise { get; set; }
        public Equipe? Equipe { get; set; }    
    }
}
