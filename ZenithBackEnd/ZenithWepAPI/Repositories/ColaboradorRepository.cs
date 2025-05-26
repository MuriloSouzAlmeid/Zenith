using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly ZenithContext _context;

        public ColaboradorRepository()
        {
            _context = new ZenithContext();
        }

        public Colaborador BuscarPeloId(Guid idColaborador)
        {
            return _context.Colaborador.FirstOrDefault(c => c.Id == idColaborador);
        }

        public Colaborador BuscarPorIdUsuario(Guid idUsuario)
        {
            Colaborador colaboradorBuscado = _context.Colaborador.FirstOrDefault(c => c.IdUsuario == idUsuario)!;

            return colaboradorBuscado;
        }

        public void Cadastrar(Colaborador novoColaborador)
        {
           _context.Colaborador.Add(novoColaborador);

            _context.SaveChanges();
        }

        public void Deletar(Guid idColaborador)
        {
            Colaborador colaboradorBuscado = _context.Colaborador.FirstOrDefault(colaborador => colaborador.Id == idColaborador)!;

            if(colaboradorBuscado != null)
            {
                _context.Colaborador.Remove(colaboradorBuscado);

                _context.SaveChanges();
            }
        }

        public List<Colaborador> ListarTodos()
        {
            List<Colaborador> listaDeColaboradores = _context.Colaborador.Include(colaborador => colaborador.Usuario).ToList();

            return listaDeColaboradores;
        }
    }
}
