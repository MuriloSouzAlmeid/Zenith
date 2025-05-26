using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class FuncionalidadeRepository : IFuncionalidadeRepository
    {
        private readonly ZenithContext _context;

        public FuncionalidadeRepository()
        {
            _context = new ZenithContext();
        }

        public Funcionalidade BuscarPorId(Guid idFuncionalidade)
        {
            return _context.Funcionalidade.FirstOrDefault(f => f.Id == idFuncionalidade);
        }

        public Funcionalidade BuscarPorNome(string nomeFuncionalidade)
        {
            return _context.Funcionalidade.FirstOrDefault(f => f.Descricao.ToLower() == nomeFuncionalidade.ToLower());
        }

        public void Cadastrar(Funcionalidade novaFuncionalidade)
        {
            _context.Funcionalidade.Add(novaFuncionalidade);

            _context.SaveChanges();
        }

        public List<Funcionalidade> ListarTodas()
        {
            return _context.Funcionalidade.ToList();
        }
    }
}
