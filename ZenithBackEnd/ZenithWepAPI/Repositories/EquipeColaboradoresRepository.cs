using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class EquipeColaboradoresRepository : IEquipeColaboradoresRepository
    {
        private readonly ZenithContext _context;

        public EquipeColaboradoresRepository()
        {
            _context = new ZenithContext();
        }

        public void Deletar(Guid idEquipeColaborador)
        {
            EquipeColaboradores registroBuscado = _context.EquipeColaboradores.FirstOrDefault(ec => ec.Id == idEquipeColaborador);

            if(registroBuscado != null ) 
            {
                _context.EquipeColaboradores.Remove(registroBuscado);

                _context.SaveChanges();
            }
        }

        public List<EquipeColaboradores> ListarPelaEquipe(Guid idEquipe)
        {
            return _context.EquipeColaboradores
                .Include(ec => ec.Colaborador)
                .Include(ec => ec.Equipe)
                .Where(ec => ec.IdEquipe == idEquipe)
                .ToList();
        }

        public List<EquipeColaboradores> ListarPeloColaborador(Guid idColaborador)
        {
            return _context.EquipeColaboradores
                .Include(ec => ec.Colaborador)
                .Include(ec => ec.Equipe)
                .Where(ec => ec.IdColaborador == idColaborador)
                .ToList();
        }
    }
}
