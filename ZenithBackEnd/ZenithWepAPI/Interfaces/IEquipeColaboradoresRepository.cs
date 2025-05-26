using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IEquipeColaboradoresRepository
    {
        List<EquipeColaboradores> ListarPeloColaborador(Guid idColaborador);
        List<EquipeColaboradores> ListarPelaEquipe(Guid idEquipe);
        void Deletar(Guid idEquipeColaborador);
    }
}
