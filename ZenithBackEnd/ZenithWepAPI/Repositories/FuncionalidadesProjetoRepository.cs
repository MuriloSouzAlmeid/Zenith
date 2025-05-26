using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class FuncionalidadesProjetoRepository : IFuncionalidadesProjetoRepository
    {
        private readonly ZenithContext _context;

        public FuncionalidadesProjetoRepository()
        {
            _context = new ZenithContext();
        }

        public FuncionalidadesProjeto BuscarPorId(Guid idFuncionalidadeProjeto)
        {
            FuncionalidadesProjeto funcionalidadeProjetoBuscada = _context.FuncionalidadesProjeto.FirstOrDefault(f => f.Id == idFuncionalidadeProjeto);

            return funcionalidadeProjetoBuscada;
        }

        public void Cadastrar(FuncionalidadesProjeto novaFuncionalidadeProjeto)
        {
            _context.FuncionalidadesProjeto.Add(novaFuncionalidadeProjeto);

            _context.SaveChanges();
        }

        public void Deletar(Guid idFuncionalidadeProjeto)
        {
            FuncionalidadesProjeto funcionalidadeBuscada = this.BuscarPorId(idFuncionalidadeProjeto);

            if (funcionalidadeBuscada != null) 
            {
                _context.FuncionalidadesProjeto.Remove(funcionalidadeBuscada);

                _context.SaveChanges();
            } 
        }

        public List<FuncionalidadesProjeto> ListarPorIdProjeto(Guid idProjeto)
        {
            return _context.FuncionalidadesProjeto
                .Include(f => f.Funcionalidade)
                .Where(f => f.IdProjeto == idProjeto)
                .ToList();
        }

        public List<FuncionalidadesProjeto> ListarTodas()
        {
            return _context.FuncionalidadesProjeto
                .Include(f => f.Funcionalidade)
                .Include(f => f.Projeto)
                .ToList();
        }
    }
}
