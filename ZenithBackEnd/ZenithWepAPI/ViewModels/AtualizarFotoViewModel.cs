using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ZenithWepAPI.ViewModels
{
    public class AtualizarFotoViewModel
    {
        [NotMapped]
        [JsonIgnore]
        public IFormFile ArquivoFoto { get; set; }
    }
}
