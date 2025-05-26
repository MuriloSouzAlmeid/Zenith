using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IFuncionalidadesProjetoRepository
    {
        FuncionalidadesProjeto BuscarPorId(Guid idFuncionalidadeProjeto);
        void Cadastrar(FuncionalidadesProjeto novaFuncionalidadeProjeto);
        List<FuncionalidadesProjeto> ListarPorIdProjeto(Guid idProjeto);
        void Deletar(Guid idFuncionalidadeProjeto);
        List<FuncionalidadesProjeto> ListarTodas();
    }
}
