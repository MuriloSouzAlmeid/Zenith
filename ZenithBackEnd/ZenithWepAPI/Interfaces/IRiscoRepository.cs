using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IRiscoRepository
    {
        void Cadastrar(Risco novoRisco);
        Risco BuscarPeloId(Guid idRisco);
        void Deletar(Guid idRisco);
        List<Risco> ListarPelaAnalise(Guid idAnalise);
    }
}
