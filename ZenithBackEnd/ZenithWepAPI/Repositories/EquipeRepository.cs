using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;

namespace ZenithWepAPI.Repositories
{
    public class EquipeRepository : IEquipeRepository
    {
        private readonly ZenithContext _context;

        public EquipeRepository()
        {
            _context = new ZenithContext();
        }

        // Cadastrar uma nova equipe
        public void Cadastrar(Equipe novaEquipe)
        {
            _context.Equipe.Add(novaEquipe);
            _context.SaveChanges();
        }

        // Deletar uma equipe
        public void Deletar(Guid idEquipe)
        {
            Equipe  equipe = _context.Equipe.FirstOrDefault(e => e.Id == idEquipe);
            
            if (equipe != null)
            {
                _context.Equipe.Remove(equipe);
                _context.SaveChanges();
            }
        }

        // Atualizar os dados de uma equipe
        public Equipe Atualizar(Guid idEquipe, Equipe equipeAtualizada)
        {
            var equipeExistente = _context.Equipe.FirstOrDefault(equipe => equipe.Id == idEquipe);

            if (equipeExistente != null)
            {
                if (equipeAtualizada.QtdIntegrantes != null)
                {
                    equipeExistente.QtdIntegrantes = equipeAtualizada.QtdIntegrantes;
                }

                if (equipeAtualizada.IdProjeto != null)
                {
                    equipeExistente.IdProjeto = equipeAtualizada.IdProjeto;
                }

                _context.SaveChanges();
            }

            return equipeExistente!;
        }

        // Buscar equipe por ID, incluindo os colaboradores através da tabela intermediária
        public Equipe BuscarPorId(Guid idEquipe)
        {
            return _context.Equipe
                .Include(e => e.Projeto)
                // Incluindo a tabela intermediária EquipeColaboradores
                .Include(e => e.EquipeColaboradores)
                .ThenInclude(ec => ec.Colaborador) // Incluindo os colaboradores associados à equipe
                .FirstOrDefault(e => e.Id == idEquipe)!;
        }

        // Listar todas as equipes de um projeto específico, incluindo os colaboradores
        public Equipe BuscarPorIdProjeto(Guid idProjeto)
        {
            return _context.Equipe
                .Include(e => e.Projeto)
                .Include(e => e.EquipeColaboradores)  // Incluindo a tabela intermediária
                .ThenInclude(ec => ec.Colaborador)    // Incluindo os colaboradores
                .FirstOrDefault(e => e.IdProjeto == idProjeto);
        }

        // Método para adicionar um colaborador à equipe, usando a tabela intermediária
        public void AdicionarColaborador(Guid idEquipe, Guid idColaborador)
        {
            // Verifica se a associação já existe
            var existeAssociacao = _context.EquipeColaboradores
                .Any(ec => ec.IdEquipe == idEquipe && ec.IdColaborador == idColaborador);

            if (existeAssociacao)
            {
                return; // Caso já exista, não faz nada
            }

            var equipeColaborador = new EquipeColaboradores
            {
                IdEquipe = idEquipe,
                IdColaborador = idColaborador
            };

            _context.EquipeColaboradores.Add(equipeColaborador);
            _context.SaveChanges();
        }

        // Método para remover um colaborador da equipe, removendo o registro da tabela intermediária
        public void RemoverColaborador(Guid idEquipe, Guid idColaborador)
        {
            var equipeColaborador = _context.EquipeColaboradores
                .FirstOrDefault(ec => ec.IdEquipe == idEquipe && ec.IdColaborador == idColaborador);

            if (equipeColaborador != null)
            {
                _context.EquipeColaboradores.Remove(equipeColaborador);
                _context.SaveChanges();
            }
        }

        // Listar todas as equipes cadastradas
        public List<Equipe> ListarTodasEquipes()
        {
            try
            {
                return _context.Equipe.ToList();
            }
            catch (Exception erro)
            {
                throw new Exception("Erro ao listar as equipes: " + erro.Message);
            }
        }

        public List<Colaborador> ListarColaboradoresPorEquipe(Guid idEquipe)
        {
            try
            {
                // Consulta na tabela intermediária para buscar os colaboradores associados à equipe
                var colaboradores = _context.EquipeColaboradores
                    .Where(ec => ec.IdEquipe == idEquipe)        // Filtra os registros pela equipe
                    .Include(ec => ec.Colaborador)               // Inclui os dados do colaborador
                        .ThenInclude(c => c.Usuario)             // Inclui os dados do usuário do colaborador
                            .ThenInclude(u => u.CargoUsuario)    // Inclui os dados do cargo do usuário
                    .Select(ec => ec.Colaborador!)               // Seleciona os colaboradores
                    .ToList();

                return colaboradores; // Retorna a lista de colaboradores
            }
            catch (Exception erro)
            {
                throw new Exception($"Erro ao listar colaboradores da equipe {idEquipe}: {erro.Message}");
            }
        }
    }
}
