using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class TechSkillRepository : ITechSkillRepository
    {
        private readonly ZenithContext _context;

        public TechSkillRepository()
        {
            _context = new ZenithContext();
        }

        public TechSkill BuscarPorId(Guid idSkill)
        {
            TechSkill skillBuscada = _context.TechSkill.FirstOrDefault(skill => skill.Id == idSkill)!;
            
            return skillBuscada;
        }

        public TechSkill BuscarPorNome(string nomeSkill)
        {
            TechSkill skillBuscada = _context.TechSkill.FirstOrDefault(skill => skill.Skill.ToLower() == nomeSkill.ToLower())!;

            return skillBuscada;
        }

        public void Cadastrar(TechSkill novaSkill)
        {
            _context.TechSkill.Add(novaSkill);

            _context.SaveChanges();
        }

        public List<TechSkill> ListarTodas()
        {
            return _context.TechSkill.ToList();
        }
    }
}
