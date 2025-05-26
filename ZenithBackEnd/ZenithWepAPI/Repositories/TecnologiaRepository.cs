using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class TecnologiaRepository : ITecnologiaRepository
    {
        private readonly ZenithContext _context;
        public TecnologiaRepository()
        {
            _context = new ZenithContext();
        }

        public Tecnologia BuscarPorId(Guid idTecnologia)
        {
            return _context.Tecnologia.FirstOrDefault(t => t.Id == idTecnologia);
        }

        public Tecnologia BuscarPorNome(string nomeTecnologia)
        {
            return _context.Tecnologia.FirstOrDefault(t => t.NomeTecnologia.ToLower() == nomeTecnologia.ToLower());
        }

        public void Cadastrar(Tecnologia novaTecnologia)
        {
            _context.Tecnologia.Add(novaTecnologia);

            _context.SaveChanges();
        }

        public List<Tecnologia> ListarTodas()
        {
            return _context.Tecnologia.ToList();
        }
    }
}
