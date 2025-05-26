using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IUsuarioRepository
    {
        void Cadastrar(Usuario novoUsuario);
        Usuario BuscarPorId(Guid id);
        Usuario BuscarPorLogin(string email, string senha);
        List<Usuario> ListarTodos();
        void Deletar(Guid idUsuario);
        void AtualizarFotoUsuario(string novaUrl, Guid idUsuario);
        void AtualizarUsuario(Guid idUsuario, Usuario usuarioAtualizado);
        List<Usuario> ListarPorCargo(string cargoUsuario);
        List<Usuario> ListarNaoColaboradores();
        List<Usuario> ListarColaboradores();
        Usuario BuscarPorEmail(string email);
        List<Usuario> ListarPorAreaCargo(string area);
    }
}
