using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class RiscoRepository : IRiscoRepository
    {
        private readonly ZenithContext _context;

        public RiscoRepository()
        {
            _context = new ZenithContext();
        }

        public Risco BuscarPeloId(Guid idRisco)
        {
            return _context.Risco.FirstOrDefault(r => r.Id == idRisco)!;
        }

        public void Cadastrar(Risco novoRisco)
        {
            _context.Risco.Add(novoRisco);

            _context.SaveChanges();
        }

        public void Deletar(Guid idRisco)
        {
            Risco riscoBuscado =_context.Risco.FirstOrDefault(r => r.Id == idRisco);

            if (riscoBuscado != null) 
            {
                _context.Risco.Remove(riscoBuscado);
            }

            _context.SaveChanges();
        }

        public List<Risco> ListarPelaAnalise(Guid idAnalise)
        {
            return _context.Risco.Where(r => r.IdAnaliseProjeto == idAnalise).ToList();
        }
    }
}
