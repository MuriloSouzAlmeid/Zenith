using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly ZenithContext _context;
        public ProjetoRepository()
        {
            _context = new ZenithContext();
        }

        public Projeto Atualizar(Guid idProjeto, Projeto infoProjeto)
        {
            Projeto projetoBuscado = this.BuscarPorId(idProjeto);

            if(projetoBuscado != null)
            {
                if (projetoBuscado.Nome != infoProjeto.Nome && infoProjeto.Nome != null)
                {
                    projetoBuscado.Nome = infoProjeto.Nome;
                }

                if (projetoBuscado.Descricao != infoProjeto.Descricao && infoProjeto.Descricao != null)
                {
                    projetoBuscado.Descricao = infoProjeto.Descricao;
                }

                if (projetoBuscado.DataInicio != infoProjeto.DataInicio && infoProjeto.DataInicio != null)
                {
                    projetoBuscado.DataInicio = infoProjeto.DataInicio;
                }

                if (projetoBuscado.DataFinal != infoProjeto.DataFinal && infoProjeto.DataFinal != null)
                {
                    projetoBuscado.DataFinal = infoProjeto.DataFinal;
                }

                if(projetoBuscado.IdTipoProjeto != infoProjeto.IdTipoProjeto)
                {
                    projetoBuscado.IdTipoProjeto = infoProjeto.IdTipoProjeto;
                }

                _context.Projeto.Update(projetoBuscado);

                _context.SaveChanges();
            }

            return projetoBuscado;
        }

        public void AtualizarNivel(Guid idProjeto, int NivelProjeto)
        {
            Projeto projetoBuscado = this.BuscarPorId(idProjeto);

            if(projetoBuscado != null)
            {
                if (projetoBuscado.NivelProjeto != NivelProjeto)
                {
                    projetoBuscado.NivelProjeto = NivelProjeto;
                }

                _context.Projeto.Update(projetoBuscado);

                _context.SaveChanges();
            }
        }

        public void AtualizarNivelAnalise(Guid idProjeto, int nivelProjeto)
        {
            Projeto projetoBuscado = this.BuscarPorId(idProjeto);

            if (projetoBuscado != null)
            {
                if (projetoBuscado.NivelAnalise != nivelProjeto)
                {
                    projetoBuscado.NivelAnalise = nivelProjeto;
                }

                _context.Projeto.Update(projetoBuscado);

                _context.SaveChanges();
            }
        }

        public Projeto BuscarPorId(Guid idProjeto)
        {
            Projeto projetoBusado = _context.Projeto
                .Include(p => p.TipoProjeto)
                .FirstOrDefault(projeto => projeto.Id == idProjeto)!;

            if (projetoBusado != null) {
                return projetoBusado;
            }

            return null;
        }

        public Projeto BuscarPorIdEquipe(Guid IdEquipe)
        {
            Projeto projetoBusado = _context.Projeto.Include(projeto => projeto.Equipe)
                .Include(projeto => projeto.TipoProjeto)
                .FirstOrDefault(projeto => projeto.Equipe!.Id == IdEquipe)!;

            if (projetoBusado != null)
            {
                return projetoBusado;
            }

            return null;
        }

        public void Cadastrar(Projeto novoProjeto)
        {
            _context.Projeto.Add(novoProjeto);
            _context.SaveChanges();
        }

        public void Deletar(Guid idProjeto)
        {
            Projeto projetoBuscado = this.BuscarPorId(idProjeto);

            if (projetoBuscado != null)
            {
                _context.Projeto.Remove(projetoBuscado);

                _context.SaveChanges();
            }
        }

        public List<Projeto> ListaPorColaborador(Guid idColaborador)
        {
            List<Projeto> listaDeProjetos = new List<Projeto>();

            List<EquipeColaboradores> listaDeEquipesColaboradores = _context.EquipeColaboradores
                .Where(equipeColaborador => equipeColaborador.IdColaborador == idColaborador)
                .ToList();

            if(listaDeEquipesColaboradores.Count > 0)
            {
                foreach(EquipeColaboradores equipeColaborador in listaDeEquipesColaboradores)
                {
                    Projeto projetoBuscadoPelaEquipe = this.BuscarPorIdEquipe(equipeColaborador.IdEquipe);

                    if(projetoBuscadoPelaEquipe != null)
                    {
                        listaDeProjetos.Add(projetoBuscadoPelaEquipe);
                    }
                }
            }

            return listaDeProjetos;
        }

        public List<Projeto> ListaPorNivel(int nivelProjeto, Guid idUsuario)
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => projeto.NivelProjeto == nivelProjeto && projeto.IdUsuario == idUsuario)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListaPorNivel(int nivelProjeto)
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => projeto.NivelProjeto == nivelProjeto)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListaPorNivelColaborador(int nivelProjeto, Guid idColaborador)
        {
            List<Projeto> listaDeProjetos = new List<Projeto>();

            List<EquipeColaboradores> listaDeEquipesColaboradores = _context.EquipeColaboradores
                .Where(equipeColaborador => equipeColaborador.IdColaborador == idColaborador)
                .ToList();

            if (listaDeEquipesColaboradores.Count > 0)
            {
                foreach (EquipeColaboradores equipeColaborador in listaDeEquipesColaboradores)
                {
                    Projeto projetoBuscadoPelaEquipe = this.BuscarPorIdEquipe(equipeColaborador.IdEquipe);

                    if (projetoBuscadoPelaEquipe != null && projetoBuscadoPelaEquipe.NivelProjeto == nivelProjeto)
                    {
                        listaDeProjetos.Add(projetoBuscadoPelaEquipe);
                    }
                }
            }

            return listaDeProjetos;
        }

        public List<Projeto> ListarPorTipo(string tipoProjeto)
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => projeto.TipoProjeto.Tipo == tipoProjeto)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarPorTipo(string tipoProjeto, Guid idUsuario)
        {
            
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => projeto.TipoProjeto.Tipo == tipoProjeto && projeto.IdUsuario == idUsuario)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarPorUsuario(Guid idUsuario)
        {
            List<Projeto> listaDeProjetos = _context.Projeto
                .Include(p => p.TipoProjeto)
                .Where(projeto => projeto.IdUsuario == idUsuario)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosConcluidos(Guid idUsuario)
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => (projeto.DataFinal < DateTime.Now) && projeto.IdUsuario == idUsuario)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosConcluidos()
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => projeto.DataFinal < DateTime.Now)
                .ToList();

            return listaDeProjetos;

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosConcluidosColaborador(Guid idColaborador)
        {
            List<Projeto> listaDeProjetos = new List<Projeto>();

            List<EquipeColaboradores> listaDeEquipesColaboradores = _context.EquipeColaboradores
                .Where(equipeColaborador => equipeColaborador.IdColaborador == idColaborador)
                .ToList();

            if (listaDeEquipesColaboradores.Count > 0)
            {
                foreach (EquipeColaboradores equipeColaborador in listaDeEquipesColaboradores)
                {
                    Projeto projetoBuscadoPelaEquipe = this.BuscarPorIdEquipe(equipeColaborador.IdEquipe);

                    if (projetoBuscadoPelaEquipe != null && (projetoBuscadoPelaEquipe.DataFinal < DateTime.Now))
                    {
                        listaDeProjetos.Add(projetoBuscadoPelaEquipe);
                    }
                }
            }

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosEmAndamento(Guid idUsuario)
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => (projeto.DataFinal > DateTime.Now && projeto.DataInicio < DateTime.Now) && projeto.IdUsuario == idUsuario)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosEmAndamento()
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => (projeto.DataFinal > DateTime.Now && projeto.DataInicio < DateTime.Now))
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosEmAndamentoColaborador(Guid idColaborador)
        {
            List<Projeto> listaDeProjetos = new List<Projeto>();

            List<EquipeColaboradores> listaDeEquipesColaboradores = _context.EquipeColaboradores
                .Where(equipeColaborador => equipeColaborador.IdColaborador == idColaborador)
                .ToList();

            if (listaDeEquipesColaboradores.Count > 0)
            {
                foreach (EquipeColaboradores equipeColaborador in listaDeEquipesColaboradores)
                {
                    Projeto projetoBuscadoPelaEquipe = this.BuscarPorIdEquipe(equipeColaborador.IdEquipe);

                    if (projetoBuscadoPelaEquipe != null && (projetoBuscadoPelaEquipe.DataFinal > DateTime.Now && projetoBuscadoPelaEquipe.DataInicio < DateTime.Now))
                    {
                        listaDeProjetos.Add(projetoBuscadoPelaEquipe);
                    }
                }
            }

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosFuturos(Guid idUsuario)
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => (projeto.DataInicio > DateTime.Now) && projeto.IdUsuario == idUsuario)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosFuturos()
        {
            List<Projeto> listaDeProjetos = _context.Projeto.Include(p => p.TipoProjeto)
                .Where(projeto => (projeto.DataInicio > DateTime.Now))
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosFuturosColaborador(Guid idColaborador)
        {
            List<Projeto> listaDeProjetos = new List<Projeto>();

            List<EquipeColaboradores> listaDeEquipesColaboradores = _context.EquipeColaboradores
                .Where(equipeColaborador => equipeColaborador.IdColaborador == idColaborador)
                .ToList();

            if (listaDeEquipesColaboradores.Count > 0)
            {
                foreach (EquipeColaboradores equipeColaborador in listaDeEquipesColaboradores)
                {
                    Projeto projetoBuscadoPelaEquipe = this.BuscarPorIdEquipe(equipeColaborador.IdEquipe);

                    if (projetoBuscadoPelaEquipe != null && (projetoBuscadoPelaEquipe.DataInicio > DateTime.Now))
                    {
                        listaDeProjetos.Add(projetoBuscadoPelaEquipe);
                    }
                }
            }

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosQueNaoEstaoEmAndamento()
        {
            List<Projeto> listaDeProjetos = _context.Projeto
                .Include(projeto => projeto.TipoProjeto)
                .Where(projeto => (projeto.DataInicio > DateTime.Now) || (projeto.DataFinal < DateTime.Now))
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarProjetosQueNaoEstaoEmAndamento(Guid idUsuario)
        {
            List<Projeto> listaDeProjetos = _context.Projeto
                .Include(projeto => projeto.TipoProjeto)
                .Where(projeto => (projeto.DataInicio > DateTime.Now) || (projeto.DataFinal < DateTime.Now) && projeto.IdUsuario == idUsuario)
                .ToList();

            return listaDeProjetos;
        }

        public List<Projeto> ListarTodos()
        {
            List<Projeto> listaDeProjetos = _context.Projeto
                .Include(projeto => projeto.TipoProjeto)
                .ToList();

            return listaDeProjetos;
        }
    }
}
