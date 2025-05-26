using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class ColaboradorTechSkillsRepository : IColaboradorTechSkillsRepository
    {
        private readonly ZenithContext _context;

        public ColaboradorTechSkillsRepository()
        {
            _context = new ZenithContext();
        }

        public void Cadastrar(ColaboradorTechSkills novoColaboradorTechSkills)
        {
            _context.ColaboradorTechSkills.Add(novoColaboradorTechSkills);

            _context.SaveChanges();
        }

        public void Deletar(Guid idColaboradorTechSkill)
        {
            ColaboradorTechSkills registroBuscado = _context.ColaboradorTechSkills.FirstOrDefault(registro => registro.Id == idColaboradorTechSkill)!;

            if(registroBuscado != null)
            {
                _context.ColaboradorTechSkills.Remove(registroBuscado);

                _context.SaveChanges();
            }
        }

        public List<ColaboradorTechSkills> ListarPorTechSkill(string[] nomesTechSkill)
        {
            List<ColaboradorTechSkills> lista = _context.ColaboradorTechSkills.Include(x => x.TechSkill)
                .Include(x => x.Colaborador)
                .Where(x => nomesTechSkill.Contains(x.TechSkill!.Skill)).ToList();

            return lista;
        }

        public List<ColaboradorTechSkills> ListarPorColaborador(Guid idColaborador)
        {
            List<ColaboradorTechSkills> lista = _context.ColaboradorTechSkills.Include(x => x.TechSkill)
                .Where(x => x.IdColaborador == idColaborador).ToList();

            return lista;
        }
    }
}
