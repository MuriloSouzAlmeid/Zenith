using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IEquipeRepository
    {
        void Cadastrar(Equipe novaEquipe);
        void Deletar(Guid idEquipe);
        Equipe Atualizar(Guid idEquipe, Equipe equipeAtualizada);
        Equipe BuscarPorId(Guid idEquipe);
        Equipe BuscarPorIdProjeto(Guid idProjeto);

        List<Equipe> ListarTodasEquipes();

        // Métodos relacionados à tabela intermediária "EquipeColaboradores"
        void AdicionarColaborador(Guid idEquipe, Guid idColaborador);
        void RemoverColaborador(Guid idEquipe, Guid idColaborador);
        List<Colaborador> ListarColaboradoresPorEquipe(Guid idEquipe);
    }
}
