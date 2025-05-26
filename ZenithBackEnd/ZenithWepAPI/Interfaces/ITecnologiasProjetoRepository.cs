using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface ITecnologiasProjetoRepository
    {
        TecnologiasProjeto BuscarPorId(Guid idTecnologiaProjeto);
        void Cadastrar(TecnologiasProjeto novaTecnologiaProjeto);
        List<TecnologiasProjeto> ListarPorIdProjeto(Guid idProjeto);
        void Deletar(Guid idTecnologiaProjeto);
        List<TecnologiasProjeto> ListarTodas();
    }
}
