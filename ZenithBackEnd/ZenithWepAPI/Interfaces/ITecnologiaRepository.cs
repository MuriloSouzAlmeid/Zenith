using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface ITecnologiaRepository
    {
        Tecnologia BuscarPorId(Guid idTecnologia);
        Tecnologia BuscarPorNome(string nomeTecnologia);
        void Cadastrar(Tecnologia novaTecnologia);
        List<Tecnologia> ListarTodas();
    }
}
