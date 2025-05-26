using System.Reflection.Metadata.Ecma335;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class TipoProjetoRepository : ITipoProjetoRepository
    {
        private readonly ZenithContext _context;

        public TipoProjetoRepository()
        {
            _context = new ZenithContext();
        }

        public TipoProjeto BuscarPorId(Guid idTipoProjeto)
        {
            return _context.TiposProjeto.FirstOrDefault(tp => tp.Id == idTipoProjeto);
        }

        public TipoProjeto BuscarPorNome(string nomeTipoProjeto)
        {
            return _context.TiposProjeto.FirstOrDefault(tp => tp.Tipo.ToLower() == nomeTipoProjeto.ToLower());
        }

        public void Cadastrar(TipoProjeto novoTipoProjeto)
        {
            _context.TiposProjeto.Add(novoTipoProjeto);

            _context.SaveChanges();
        }

        public List<TipoProjeto> ListarTodas()
        {
            return _context.TiposProjeto.ToList();
        }
    }
}
