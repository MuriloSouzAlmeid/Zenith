using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface IColaboradorTechSkillsRepository
    {
        void Cadastrar(ColaboradorTechSkills novoColaboradorTechSkills);
        void Deletar(Guid idColaboradorTechSkill);
        List<ColaboradorTechSkills> ListarPorColaborador(Guid idColaborador);
        List<ColaboradorTechSkills> ListarPorTechSkill(string[] nomeTechSkill);
    }
}
