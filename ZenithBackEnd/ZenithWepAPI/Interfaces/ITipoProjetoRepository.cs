using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface ITipoProjetoRepository
    {
        TipoProjeto BuscarPorId(Guid idTipoProjeto);
        TipoProjeto BuscarPorNome(string nomeTipoProjeto);
        void Cadastrar(TipoProjeto novoTipoProjeto);
        List<TipoProjeto> ListarTodas();
    }
}
