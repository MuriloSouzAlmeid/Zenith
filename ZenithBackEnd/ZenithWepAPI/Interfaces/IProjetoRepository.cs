using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IProjetoRepository
    {
        void Cadastrar(Projeto novoProjeto);
        void Deletar(Guid idProjeto);
        Projeto Atualizar(Guid idProjeto, Projeto infoProjeto);
        void AtualizarNivel(Guid idProjeto, int NivelProjeto);
        void AtualizarNivelAnalise(Guid idProjeto, int nivelProjeto);
        Projeto BuscarPorId(Guid idProjeto);
        Projeto BuscarPorIdEquipe(Guid IdEquipe);
        List<Projeto> ListarTodos();
        List<Projeto> ListarPorUsuario(Guid idUsuario);
        List<Projeto> ListarPorTipo(string tipoProjeto);
        List<Projeto> ListarPorTipo(string tipoProjeto, Guid idUsuario);
        List<Projeto> ListaPorNivel(int nivelProjeto);
        List<Projeto> ListaPorNivel(int nivelProjeto, Guid idUsuario);
        List<Projeto> ListaPorNivelColaborador(int nivelProjeto, Guid idColaborador);
        List<Projeto> ListaPorColaborador(Guid idColaborador);
        List<Projeto> ListarProjetosQueNaoEstaoEmAndamento();
        List<Projeto> ListarProjetosQueNaoEstaoEmAndamento(Guid idUsuario);
        List<Projeto> ListarProjetosEmAndamento();
        List<Projeto> ListarProjetosConcluidos();
        List<Projeto> ListarProjetosFuturos();
        List<Projeto> ListarProjetosEmAndamento(Guid idUsuario);
        List<Projeto> ListarProjetosConcluidos(Guid idUsuario);
        List<Projeto> ListarProjetosFuturos(Guid idUsuario);
        List<Projeto> ListarProjetosEmAndamentoColaborador(Guid idColaborador);
        List<Projeto> ListarProjetosConcluidosColaborador(Guid idColaborador);
        List<Projeto> ListarProjetosFuturosColaborador(Guid idColaborador);
    }
}
