using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class TecnologiasProjetoRepository : ITecnologiasProjetoRepository
    {
        private readonly ZenithContext _context;
        public TecnologiasProjetoRepository()
        {
            _context = new ZenithContext();
        }

        public TecnologiasProjeto BuscarPorId(Guid idTecnologiaProjeto)
        {
            return _context.TecnologiasProjeto.FirstOrDefault(t => t.Id == idTecnologiaProjeto);
        }

        public void Cadastrar(TecnologiasProjeto novaTecnologiaProjeto)
        {
            _context.TecnologiasProjeto.Add(novaTecnologiaProjeto);

            _context.SaveChanges();
        }

        public void Deletar(Guid idTecnologiaProjeto)
        {
            TecnologiasProjeto tecnologiaBuscada = this.BuscarPorId(idTecnologiaProjeto);

            if(tecnologiaBuscada != null)
            {
                _context.TecnologiasProjeto.Remove(tecnologiaBuscada);

                _context.SaveChanges();
            }
        }

        public List<TecnologiasProjeto> ListarPorIdProjeto(Guid idProjeto)
        {
            return _context.TecnologiasProjeto
                .Include(t => t.Tecnologia)
                .Where(t => t.IdProjeto == idProjeto)
                .ToList();
        }

        public List<TecnologiasProjeto> ListarTodas()
        {
            return _context.TecnologiasProjeto
                .Include(t => t.Tecnologia)
                .Include(t => t.Projeto)
                .ToList();
        }
    }
}
