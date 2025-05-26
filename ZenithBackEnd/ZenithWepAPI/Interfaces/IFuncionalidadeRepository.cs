using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IFuncionalidadeRepository
    {
        Funcionalidade BuscarPorId(Guid idFuncionalidade);
        Funcionalidade BuscarPorNome(string nomeFuncionalidade);
        void Cadastrar(Funcionalidade novaFuncionalidade);
        List<Funcionalidade> ListarTodas();
    }
}
