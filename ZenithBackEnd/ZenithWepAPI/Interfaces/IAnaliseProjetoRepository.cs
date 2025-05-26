using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IAnaliseProjetoRepository
    {
        void CadastrarAnalise(AnaliseProjeto novaAnalise);
        AnaliseProjeto BuscarPeloId(Guid idAnalise);
        AnaliseProjeto BuscarPeloIdProjeto(Guid idProjeto);
        void Deletar(Guid idAnalise);
        void AtualizarAnalise(Guid idAnalise, AnaliseProjeto novaAnalise);
    }
}
