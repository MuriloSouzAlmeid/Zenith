using ZenithWepAPI.Domains;

namespace ZenithWepAPI.Interfaces
{
    public interface ITechSkillRepository
    {
        void Cadastrar(TechSkill novaSkill);
        TechSkill BuscarPorId(Guid idSkill);
        TechSkill BuscarPorNome(string nomeSkill);
        List<TechSkill> ListarTodas();
    }
}
