using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Runtime.CompilerServices;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;
using ZenithWepAPI.Repositories;
using ZenithWepAPI.Utils;
using ZenithWepAPI.Utils.AlgoritmoAnalise;
using ZenithWepAPI.Utils.BlobStorage;
using ZenithWepAPI.Utils.GeminiService;
using ZenithWepAPI.ViewModels;

namespace ZenithWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICargoUsuarioRepository _cargoRepository;
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly ITechSkillRepository _skillsRepository;
        private readonly IColaboradorTechSkillsRepository _colaboradorSkillsRepository;
        private readonly IEquipeColaboradoresRepository _equipeColaboradoresRepository;
        private readonly IGeminiServiceRepository _geminiService;


        public string containerName = "containerzenith";

        public string connectionString = "DefaultEndpointsProtocol=https;AccountName=blobvitalhubg16enzo;AccountKey=oE4zwTcqqxKfuErbVv7o9ETdAbHzELSZDt7o60W5hQ07zfFdTU4YuZIGtOyVKRjh3E3GzJwRnAHn+AStsOUjgA==;EndpointSuffix=core.windows.net";

        public UsuarioController()
        {
            _usuarioRepository = new UsuarioRepository();
            _cargoRepository = new CargoUsuarioRepositry();
            _colaboradorRepository = new ColaboradorRepository();
            _skillsRepository = new TechSkillRepository();
            _colaboradorSkillsRepository = new ColaboradorTechSkillsRepository();
            _equipeColaboradoresRepository = new EquipeColaboradoresRepository();
            _geminiService = new GeminiServiceRepository();
        }

        [HttpGet]
        public IActionResult ListarTodos()
        {
            try
            {
                return Ok(_usuarioRepository.ListarTodos());
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(Guid id)
        {
            try
            {
                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(id);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                return Ok(usuarioBuscado);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CadastrarUsuarioViewModel novoUsuario)
        {
            try
            {
                CargoUsuario cargoBuscado = _cargoRepository.BuscarPorNome(novoUsuario.CargoUsuario!);

                if (cargoBuscado == null)
                {
                    CargoUsuario novoCargo = new CargoUsuario()
                    {
                        Cargo = novoUsuario.CargoUsuario,
                        NivelCargo = 2,
                        Area = await _geminiService.RealizarAnaliseAreaDoCargo(novoUsuario.CargoUsuario)
                    };

                    _cargoRepository.Cadastrar(novoCargo);

                    cargoBuscado = novoCargo;
                }

                Usuario usuario = new Usuario()
                {
                    Nome = novoUsuario.Nome,
                    Email = novoUsuario.Email,
                    Senha = Criptografia.GerarHash(novoUsuario.Senha!),
                    IdCargoUsuario = cargoBuscado.Id,
                    NivelSenioridade = novoUsuario.NivelSenioridade,
                    Foto = "https://blobvitalhubg16enzo.blob.core.windows.net/containerzenith/userDefaultImage.jpg"
                };

                _usuarioRepository.Cadastrar(usuario);

                if (cargoBuscado.NivelCargo == 2)
                {
                    Colaborador novoColaborador = new Colaborador()
                    {
                        IdUsuario = usuario.Id
                    };

                    _colaboradorRepository.Cadastrar(novoColaborador);

                    if (novoUsuario.TechSkillsList != null)
                    {
                        for (int i = 0; i < novoUsuario.TechSkillsList.Length; i++)
                        {
                            TechSkill skillBuscada = _skillsRepository.BuscarPorNome(novoUsuario.TechSkillsList[i]);

                            if (skillBuscada == null)
                            {
                                skillBuscada = new TechSkill()
                                {
                                    Skill = novoUsuario.TechSkillsList[i]
                                };

                                _skillsRepository.Cadastrar(skillBuscada);
                            }

                            ColaboradorTechSkills novoColaboradorTechSkill = new ColaboradorTechSkills()
                            {
                                IdColaborador = novoColaborador.Id,
                                IdTechSkill = skillBuscada.Id
                            };

                            _colaboradorSkillsRepository.Cadastrar(novoColaboradorTechSkill);
                        }
                    }
                }

                return Ok(usuario.Id);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpDelete("Deletar/{id}")]
        public IActionResult Deletar(Guid id)
        {
            try
            {
                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(id);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                Colaborador colaboradorBuscado = _colaboradorRepository.BuscarPorIdUsuario(id);

                if (colaboradorBuscado != null)
                {
                    List<ColaboradorTechSkills> listaColaboradorTechSkills = _colaboradorSkillsRepository.ListarPorColaborador(colaboradorBuscado.Id);

                    if (listaColaboradorTechSkills.Count > 0)
                    {
                        foreach (ColaboradorTechSkills colaboradorTechSkills in listaColaboradorTechSkills)
                        {
                            _colaboradorSkillsRepository.Deletar(colaboradorTechSkills.Id);
                        }
                    }

                    List<EquipeColaboradores> colaboradoresEquipe = _equipeColaboradoresRepository.ListarPeloColaborador(colaboradorBuscado.Id);

                    if(colaboradoresEquipe.Count > 0)
                    {
                        foreach(EquipeColaboradores colaboradorEquipe in colaboradoresEquipe)
                        {
                            _equipeColaboradoresRepository.Deletar(colaboradorEquipe.Id);
                        }
                    }

                    _colaboradorRepository.Deletar(colaboradorBuscado.Id);
                }

                _usuarioRepository.Deletar(id);

                return Ok("Usuário deletado com sucesso");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpPut("AtualizarFoto/{idUsuario}")]
        public async Task<IActionResult> AtualizarFoto([FromForm] AtualizarFotoViewModel fotoAtualizada, Guid idUsuario)
        {
            try
            {
                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(idUsuario);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                string novaUrl = await AzureBlobStorageHelper.UploadImageBlobAsync(fotoAtualizada.ArquivoFoto, connectionString, containerName);

                _usuarioRepository.AtualizarFotoUsuario(novaUrl, idUsuario);

                return Ok("Foto atualizada com sucesso");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpPut("AtualizarUsuario/{idUsuario}")]
        public async Task<IActionResult> AtualizarUsuario(AtualizarUsuarioViewModel infoUsuario, Guid idUsuario)
        {
            try
            {
                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(idUsuario);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                Usuario usuarioAtualizado = new Usuario()
                {
                    Nome = infoUsuario.NovoNome,
                    NivelSenioridade = infoUsuario.NovoNivelSenioridade,
                    Email = infoUsuario.NovoEmail
                };

                if (infoUsuario.NovoCargo != null)
                {
                    CargoUsuario cargoBuscado = _cargoRepository.BuscarPorNome(infoUsuario.NovoCargo!);

                    if (cargoBuscado == null && infoUsuario.NovoCargo != null)
                    {
                        CargoUsuario novoCargo = new CargoUsuario()
                        {
                            Cargo = infoUsuario.NovoCargo,
                            NivelCargo = 2,
                            Area = await _geminiService.RealizarAnaliseAreaDoCargo(infoUsuario.NovoCargo)
                        };

                        _cargoRepository.Cadastrar(novoCargo);

                        cargoBuscado = novoCargo;
                    }

                    usuarioAtualizado.IdCargoUsuario = cargoBuscado != null ? cargoBuscado.Id : usuarioBuscado.IdCargoUsuario;

                    if (cargoBuscado != null && cargoBuscado.NivelCargo == 2)
                    {
                        // Atualiza o registro na tabela colaborador colaborador

                        Colaborador colaboradorBuscado = _colaboradorRepository.BuscarPorIdUsuario(idUsuario);

                        if (colaboradorBuscado == null)
                        {
                            Colaborador novoColaborador = new Colaborador()
                            {
                                IdUsuario = idUsuario
                            };

                            _colaboradorRepository.Cadastrar(novoColaborador);

                            colaboradorBuscado = novoColaborador;
                        }

                        // Atualiza as techskills do colaborador

                        List<ColaboradorTechSkills> lista = _colaboradorSkillsRepository.ListarPorColaborador(colaboradorBuscado.Id);

                        if (lista.Count > 0)
                        {
                            if (infoUsuario.NovasTechSkillsList == null || infoUsuario.NovasTechSkillsList.Length == 0)
                            {
                                foreach (ColaboradorTechSkills colaboradorTechSkill in lista)
                                {
                                    _colaboradorSkillsRepository.Deletar(colaboradorTechSkill.Id);
                                }
                            }
                            else
                            {

                                foreach (ColaboradorTechSkills colaboradorTechSkill in lista)
                                {
                                    TechSkill techSkill = _skillsRepository.BuscarPorId(colaboradorTechSkill.IdTechSkill);

                                    bool manterSkill = false;

                                    foreach (string novaSkill in infoUsuario.NovasTechSkillsList!)
                                    {
                                        if (novaSkill == techSkill.Skill)
                                        {
                                            manterSkill = true;
                                        }
                                    }

                                    if (!manterSkill)
                                    {
                                        //Deletar registros de skills
                                        _colaboradorSkillsRepository.Deletar(colaboradorTechSkill.Id);
                                    }
                                }
                            }
                        }

                        if (infoUsuario.NovasTechSkillsList != null)
                        {
                            foreach (string novaSkill in infoUsuario.NovasTechSkillsList)
                            {
                                bool manterSkill = false;

                                foreach (ColaboradorTechSkills colaboradorTechSkill in lista)
                                {
                                    TechSkill techSkill = _skillsRepository.BuscarPorId(colaboradorTechSkill.IdTechSkill);

                                    if (novaSkill == techSkill.Skill)
                                    {
                                        manterSkill = true;
                                    }
                                }

                                if (!manterSkill)
                                {
                                    //Cadastrar registros de skills
                                    TechSkill skillBuscada = _skillsRepository.BuscarPorNome(novaSkill);

                                    if (skillBuscada == null)
                                    {
                                        TechSkill novoRegistroSkill = new TechSkill()
                                        {
                                            Skill = novaSkill
                                        };

                                        _skillsRepository.Cadastrar(novoRegistroSkill);

                                        skillBuscada = novoRegistroSkill;
                                    }

                                    ColaboradorTechSkills novoRegistroColaboradorSkill = new ColaboradorTechSkills()
                                    {
                                        IdColaborador = colaboradorBuscado.Id,
                                        IdTechSkill = skillBuscada.Id
                                    };

                                    _colaboradorSkillsRepository.Cadastrar(novoRegistroColaboradorSkill);
                                }
                            }
                        }

                    }else if(cargoBuscado != null && cargoBuscado.NivelCargo != 2)
                    {
                        Colaborador colaboradorBuscado = _colaboradorRepository.BuscarPorIdUsuario(idUsuario);

                        List<ColaboradorTechSkills> colaboradoresTechSkills = _colaboradorSkillsRepository.ListarPorColaborador(colaboradorBuscado.Id);

                        foreach (ColaboradorTechSkills colaboradorTechSkills in colaboradoresTechSkills)
                        {
                            _colaboradorSkillsRepository.Deletar(colaboradorTechSkills.Id);
                        }

                        List<EquipeColaboradores> equipesColaborador = _equipeColaboradoresRepository.ListarPeloColaborador(colaboradorBuscado.Id);

                        foreach(EquipeColaboradores equipeColaborador in equipesColaborador)
                        {
                            _equipeColaboradoresRepository.Deletar(equipeColaborador.Id);
                        }
                    }
                }

                _usuarioRepository.AtualizarUsuario(idUsuario, usuarioAtualizado);

                return Ok("Usuário atualizado com sucesso!");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarTodosOsUsuarios")]
        public IActionResult ListarGerentesEColaboradores()
        {
            try
            {
                List<ListagemUsuarioViewModel> listaDeUsuarios = new List<ListagemUsuarioViewModel>();

                // Lista os Gerentes de Projeto e os Administradores

                List<Usuario> listaDeNaoColaboradores = _usuarioRepository.ListarNaoColaboradores();

                // cria um usuário para cada item listado

                if (listaDeNaoColaboradores.Count > 0)
                {
                    foreach (Usuario usuario in listaDeNaoColaboradores)
                    {
                        if(usuario.CargoUsuario!.NivelCargo != 0)
                        {
                            ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                            {
                                IdUsuario = usuario.Id,
                                Nome = usuario.Nome,
                                Email = usuario.Email,
                                Foto = usuario.Foto,
                                Cargo = usuario.CargoUsuario!.Cargo,
                                Perfil = usuario.CargoUsuario!.Cargo,
                                Senioridade = (usuario.NivelSenioridade == 1) ? "Júnior" : ((usuario.NivelSenioridade == 2) ? "Pleno" : ((usuario.NivelSenioridade == 3) ? "Sênior" : "Gestor")),
                                TechSkills = new List<string>()
                            };

                            listaDeUsuarios.Add(usuarioViewModel);
                        }
                    }
                }

                List<Usuario> listaDeColaboradores = _usuarioRepository.ListarColaboradores();

                if (listaDeColaboradores.Count > 0)
                {
                    foreach (Usuario usuario in listaDeColaboradores)
                    {
                        ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuario.Id,
                            Nome = usuario.Nome,
                            Email = usuario.Email,
                            Foto = usuario.Foto,
                            Cargo = usuario.CargoUsuario!.Cargo,
                            Perfil = "Colaborador",
                            Senioridade = (usuario.NivelSenioridade == 1) ? "Júnior" : ((usuario.NivelSenioridade == 2) ? "Pleno" : ((usuario.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        // Adicionando as TechSkills

                        Colaborador colaborador = _colaboradorRepository.BuscarPorIdUsuario(usuario.Id);

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

                        listaDeUsuarios.Add(usuarioViewModel);
                    }
                }

                return Ok(listaDeUsuarios);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarUsuariosPorCargo/{cargoUsuario}")]
        public IActionResult ListarUsuariosPorCargo(string cargoUsuario)
        {
            try
            {
                List<ListagemUsuarioViewModel> listaDeUsuarios = new List<ListagemUsuarioViewModel>();

                List<Usuario> listaDeUsuariosPorCargo = _usuarioRepository.ListarPorCargo(cargoUsuario);

                if (listaDeUsuariosPorCargo.Count > 0)
                {
                    foreach (Usuario usuario in listaDeUsuariosPorCargo)
                    {
                        ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuario.Id,
                            Nome = usuario.Nome,
                            Cargo = usuario.CargoUsuario!.Cargo,
                            Email = usuario.Email,
                            Foto = usuario.Foto,
                            Perfil = usuario.CargoUsuario!.NivelCargo == 0 ? "Administrador" : (usuario.CargoUsuario.NivelCargo == 1 ? "Gerente de Projetos" : "Colaborador"),
                            Senioridade = (usuario.NivelSenioridade == 1) ? "Júnior" : ((usuario.NivelSenioridade == 2) ? "Pleno" : ((usuario.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };


                        if (usuario.CargoUsuario.NivelCargo == 2)
                        {
                            // Adicionando as TechSkills

                            Colaborador colaborador = _colaboradorRepository.BuscarPorIdUsuario(usuario.Id);

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
                        }

                        listaDeUsuarios.Add(usuarioViewModel);
                    }
                }

                return Ok(listaDeUsuarios);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpPost("ListarPorTechSkills")]
        public IActionResult ListarPorTechSkills(string[] techSkills)
        {
            try
            {
                List<ListagemUsuarioViewModel> listaDeUsuarios = new List<ListagemUsuarioViewModel>();

                List<ColaboradorTechSkills> listaColaboradorTechSkill = _colaboradorSkillsRepository.ListarPorTechSkill(techSkills);

                if (listaColaboradorTechSkill.Count > 0)
                {
                    foreach (ColaboradorTechSkills colaboradorTechSkills in listaColaboradorTechSkill)
                    {
                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(colaboradorTechSkills.Colaborador!.IdUsuario);

                        ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            IdColaborador = colaboradorTechSkills.IdColaborador,
                            Nome = usuarioBuscado.Nome,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Perfil = "Colaborador",
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };


                        // Adicionando as TechSkills

                        List<ColaboradorTechSkills> listaDeSkillsColaborador = _colaboradorSkillsRepository.ListarPorColaborador(colaboradorTechSkills.Colaborador.Id);

                        if (listaDeSkillsColaborador.Count > 0)
                        {
                            List<string> listaDeSkills = new List<string>();

                            foreach (ColaboradorTechSkills colaboradorSkill in listaDeSkillsColaborador)
                            {
                                listaDeSkills.Add(colaboradorSkill.TechSkill.Skill);
                            }

                            usuarioViewModel.TechSkills = listaDeSkills;
                        }

                        listaDeUsuarios.Add(usuarioViewModel);
                    }
                }

                return Ok(listaDeUsuarios);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarTodosColaboradores")]
        public IActionResult GetAllColaboradores()
        {
            try
            {
                List<ListagemUsuarioViewModel> listaDeUsuarios = new List<ListagemUsuarioViewModel>();

                List<Usuario> listaDeColaboradores = _usuarioRepository.ListarColaboradores();

                if (listaDeColaboradores.Count > 0)
                {
                    foreach (Usuario usuario in listaDeColaboradores)
                    {
                        ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuario.Id,
                            Nome = usuario.Nome,
                            Email = usuario.Email,
                            Foto = usuario.Foto,
                            Cargo = usuario.CargoUsuario!.Cargo,
                            Perfil = "Colaborador",
                            Senioridade = (usuario.NivelSenioridade == 1) ? "Júnior" : ((usuario.NivelSenioridade == 2) ? "Pleno" : ((usuario.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        // Adicionando as TechSkills

                        Colaborador colaborador = _colaboradorRepository.BuscarPorIdUsuario(usuario.Id);

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

                        listaDeUsuarios.Add(usuarioViewModel);
                    }
                }

                return Ok(listaDeUsuarios);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
