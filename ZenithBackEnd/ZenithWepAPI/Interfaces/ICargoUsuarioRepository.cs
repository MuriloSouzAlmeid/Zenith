using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface ICargoUsuarioRepository
    {
        void Cadastrar(CargoUsuario novoCargo);
        void Deletar(Guid idCargo);
        CargoUsuario BuscarPorNome(string nomeCargo);
        List<CargoUsuario> ListarTodos();
    }
}
