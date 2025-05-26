using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IColaboradorRepository
    {
        void Cadastrar(Colaborador novoColaborador);
        void Deletar(Guid idColaborador);
        Colaborador BuscarPeloId(Guid idColaborador);
        Colaborador BuscarPorIdUsuario(Guid idUsuario);
        List<Colaborador> ListarTodos();
    }
}
