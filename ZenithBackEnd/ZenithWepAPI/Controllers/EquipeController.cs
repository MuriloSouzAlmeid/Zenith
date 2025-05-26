using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;
using ZenithWepAPI.Repositories;
using ZenithWepAPI.ViewModels;

namespace ZenithWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EquipeController : ControllerBase
    {
        private readonly IEquipeRepository _equipeRepository;
        private readonly IEquipeColaboradoresRepository _equipeColaboradoresRepository;
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IColaboradorTechSkillsRepository _colaboradorSkillsRepository;
        private readonly ZenithContext _context;

        public EquipeController()
        {
            _equipeRepository = new EquipeRepository();
            _colaboradorRepository = new ColaboradorRepository();
            _projetoRepository = new ProjetoRepository();
            _equipeColaboradoresRepository = new EquipeColaboradoresRepository();
            _usuarioRepository = new UsuarioRepository();
            _colaboradorSkillsRepository = new ColaboradorTechSkillsRepository();
            _context = new ZenithContext();
        }

        // Cadastrar uma nova equipe
        [HttpPost("CadastrarEquipe/{idProjeto}")]
        public IActionResult Cadastrar(Guid idProjeto, List<Guid> colaboradores)
        {
            try
            {
                // Validar se o projeto existe
                var projetoExistente = _projetoRepository.BuscarPorId(idProjeto);
                if (projetoExistente == null)
                {
                    return NotFound("Projeto não encontrado.");
                }

                //Validar se o projeto já possui equipe
                Equipe equipeExiste = _context.Equipe.FirstOrDefault(e => e.IdProjeto == projetoExistente.Id);

                if (equipeExiste != null) 
                {
                    return BadRequest("Já existe equipe cadastrada para este projeto");
                }

                // Criar uma nova equipe
                var novaEquipe = new Equipe
                {
                    IdProjeto = idProjeto,
                    QtdIntegrantes = colaboradores.Count
                };

                // Cadastrar a nova equipe
                _equipeRepository.Cadastrar(novaEquipe);

                // Adicionar os colaboradores à nova equipe
                foreach (var idColaborador in colaboradores)
                {
                    var colaboradorExistente = _colaboradorRepository.BuscarPeloId(idColaborador);
                    if (colaboradorExistente == null)
                    {
                        return NotFound($"Colaborador com ID {idColaborador} não encontrado.");
                    }

                    // Adicionar o colaborador à equipe
                    _equipeRepository.AdicionarColaborador(novaEquipe.Id, idColaborador);
                }

                // Retornar Ok com a equipe cadastrada
                return CreatedAtAction(nameof(BuscarPorId), new { idEquipe = novaEquipe.Id }, novaEquipe);
            }
            catch (Exception erro)
            {
                // Retornar erro caso haja uma exceção
                return BadRequest(erro.Message);
            }
        }

        // Atualizar os dados de uma equipe
        [HttpPut("AtualizarEquipe/{idEquipe}")]
        public IActionResult AtualizarEquipe(Guid idEquipe, List<Guid> equipeAtualizada)
        {
            try
            {
                // Verificar se a equipe existe
                var equipeExistente = _context.Equipe.FirstOrDefault(e => e.Id == idEquipe);
                if (equipeExistente == null)
                {
                    return NotFound("Equipe não encontrada.");
                }

                // Buscar os registros da tabela intermediária com o id da equipe
                var registrosIntermediarios = _context.EquipeColaboradores
                    .Where(ec => ec.IdEquipe == idEquipe)
                    .ToList();

                // Atualizar a tabela intermediária
                foreach (var registro in registrosIntermediarios)
                {
                    // Validar se o colaborador da tabela intermediária ainda está na lista recebida
                    if (!equipeAtualizada.Contains(registro.IdColaborador))
                    {
                        // Remover o colaborador que não está mais na equipe
                        _context.EquipeColaboradores.Remove(registro);
                    }
                }

                // Validar cada colaborador da lista recebida
                foreach (var idColaborador in equipeAtualizada)
                {
                    // Verificar se o colaborador existe
                    var colaboradorExistente = _context.Colaborador.FirstOrDefault(c => c.Id == idColaborador);
                    if (colaboradorExistente == null)
                    {
                        return NotFound($"Colaborador com ID {idColaborador} não encontrado.");
                    }

                    // Verificar se o colaborador já está associado à equipe
                    if (!registrosIntermediarios.Any(r => r.IdColaborador == idColaborador))
                    {
                        // Adicionar colaborador que é novo na equipe
                        var novoRegistro = new EquipeColaboradores
                        {
                            IdEquipe = idEquipe,
                            IdColaborador = idColaborador
                        };
                        _context.EquipeColaboradores.Add(novoRegistro);
                    }
                }

                // Atualizar o objeto equipe
                equipeExistente.QtdIntegrantes = equipeAtualizada.Count;

                // Salvar as alterações no banco
                _context.SaveChanges();

                return Ok("Equipe atualizada com sucesso.");
            }
            catch (Exception erro)
            {
                return BadRequest($"Erro ao atualizar equipe: {erro.Message}");
            }
        }

        // Deletar a equipe de um projeto
        [HttpDelete("DeletarEquipePorProjeto/{idProjeto}")]
        public IActionResult DeletarEquipePorProjeto(Guid idProjeto)
        {
            try
            {
                Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(idProjeto);

                if(equipeBuscada == null)
                {
                    return NotFound("Este projeto não possui equipe");
                }

                _equipeRepository.Deletar(equipeBuscada.Id);

                return Ok("Equipe deletada com sucesso");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        // Buscar equipe por ID
        [HttpGet("{idEquipe}")]
        public IActionResult BuscarPorId(Guid idEquipe)
        {
            try
            {
                var equipe = _equipeRepository.BuscarPorId(idEquipe);
                if (equipe == null)
                {
                    return NotFound("Equipe não encontrada.");
                }
                return Ok(equipe);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        // Listar equipes por ID do projeto
        [HttpGet("PorProjeto/{idProjeto}")]
        public IActionResult ListarPorIdProjeto(Guid idProjeto)
        {
            try
            {
                var equipe = _equipeRepository.BuscarPorIdProjeto(idProjeto);
                if (equipe == null)
                {
                    return NotFound("O projeto especificado não possui equipe.");
                }

                VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                {
                    Id = equipe.Id,
                    QtdIntegrantes = equipe.QtdIntegrantes,
                };

                List<EquipeColaboradores> equipeColaboradores = _equipeColaboradoresRepository.ListarPelaEquipe(equipe.Id);

                List<ListagemUsuarioViewModel> usuariosEquipe = new List<ListagemUsuarioViewModel>();

                if(equipeColaboradores != null)
                {
                    foreach(EquipeColaboradores equipeColaborador in equipeColaboradores)
                    {
                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(equipeColaborador.Colaborador.IdUsuario);

                        ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = "Colaborador",
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        // Adicionando as TechSkills

                        Colaborador colaborador = _colaboradorRepository.BuscarPorIdUsuario(usuarioBuscado.Id);

                        if (colaborador != null)
                        {
                            usuarioViewModel.IdColaborador = colaborador.Id;

                            List<ColaboradorTechSkills> listaDeSkillsColaborador = _colaboradorSkillsRepository.ListarPorColaborador(colaborador.Id);

                            if (listaDeSkillsColaborador.Count > 0)
                            {
                                List<string> listaDeSkills = new List<string>();

                                foreach (ColaboradorTechSkills colaboradorSkill in listaDeSkillsColaborador)
                                {
                                    listaDeSkills.Add(colaboradorSkill.TechSkill.Skill);
                                }

                                usuarioViewModel.TechSkills = listaDeSkills;
                            }
                        }

                        usuariosEquipe.Add(usuarioViewModel);
                    }
                }

                equipeViewModel.Colaboradores = usuariosEquipe;

                return Ok(equipeViewModel);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        // Listar todas as equipes
        [HttpGet("ListarTodas")]
        public IActionResult ListarTodas()
        {
            try
            {
                var equipes = _equipeRepository.ListarTodasEquipes();
                return Ok(equipes);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        // Adicionar um colaborador a uma equipe
        [HttpPost("AdicionarColaborador/{idEquipe}/{idColaborador}")]
        public IActionResult AdicionarColaborador(Guid idEquipe, Guid idColaborador)
        {
            try
            {
                var equipeExistente = _equipeRepository.BuscarPorId(idEquipe);
                if (equipeExistente == null)
                {
                    return NotFound("Equipe não encontrada.");
                }

                var colaboradorExistente = _colaboradorRepository.BuscarPorIdUsuario(idColaborador);
                if (colaboradorExistente == null)
                {
                    return NotFound("Colaborador não encontrado.");
                }

                _equipeRepository.AdicionarColaborador(idEquipe, idColaborador);
                return Ok("Colaborador adicionado com sucesso.");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        // Remover um colaborador de uma equipe
        [HttpDelete("RemoverColaborador/{idEquipe}/{idColaborador}")]
        public IActionResult RemoverColaborador(Guid idEquipe, Guid idColaborador)
        {
            try
            {
                var equipeExistente = _equipeRepository.BuscarPorId(idEquipe);
                if (equipeExistente == null)
                {
                    return NotFound("Equipe não encontrada.");
                }

                var colaboradorExistente = _colaboradorRepository.BuscarPorIdUsuario(idColaborador);
                if (colaboradorExistente == null)
                {
                    return NotFound("Colaborador não encontrado.");
                }

                _equipeRepository.RemoverColaborador(idEquipe, idColaborador);
                return Ok("Colaborador removido com sucesso.");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarColaboradores/{idEquipe}")]
        public IActionResult ListarColaboradoresPorEquipe(Guid idEquipe)
        {
            try
            {
                // Chamando o método no repositório
                var colaboradores = _equipeRepository.ListarColaboradoresPorEquipe(idEquipe);

                // Validando se há colaboradores associados
                if (colaboradores == null || !colaboradores.Any())
                {
                    return NotFound("Nenhum colaborador encontrado para esta equipe.");
                }

                // Retornando a lista de colaboradores
                return Ok(colaboradores);
            }
            catch (Exception erro)
            {
                // Retornando erro em caso de exceção
                return BadRequest($"Erro ao buscar colaboradores: {erro.Message}");
            }
        }
    }
}
