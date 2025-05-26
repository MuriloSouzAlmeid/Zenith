using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;
using ZenithWepAPI.Repositories;
using ZenithWepAPI.Utils.AlgoritmoAnalise;
using ZenithWepAPI.Utils.GeminiService;
using ZenithWepAPI.ViewModels;

namespace ZenithWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProjetoController : ControllerBase
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IColaboradorTechSkillsRepository _colaboradorSkillsRepository;

        private readonly IFuncionalidadeRepository _funcionalidadeRepository;
        private readonly IFuncionalidadesProjetoRepository _funcionalidadesProjetoRepository;

        private readonly ITecnologiaRepository _tecnologiaRepository;
        private readonly ITecnologiasProjetoRepository _tecnologiasProjetoRepository;

        private readonly ITipoProjetoRepository _tipoProjetoRepository;

        private readonly IEquipeRepository _equipeRepository;
        private readonly IEquipeColaboradoresRepository _equipeColaboradoresRepository;

        private readonly ITechSkillRepository _techSkillRepository;

        private readonly AlgoritimoAnaliseMethods _algoritimoAnaliseMethods;
        private readonly IGeminiServiceRepository _geminiService;

        private readonly IAnaliseProjetoRepository _analiseRepository;
        private readonly IRiscoRepository _riscoRepository;

        private readonly ZenithContext _context;
        public ProjetoController()
        {
            _projetoRepository = new ProjetoRepository();
            _usuarioRepository = new UsuarioRepository();
            _funcionalidadeRepository = new FuncionalidadeRepository();
            _funcionalidadesProjetoRepository = new FuncionalidadesProjetoRepository();
            _tecnologiaRepository = new TecnologiaRepository();
            _tecnologiasProjetoRepository = new TecnologiasProjetoRepository();
            _tipoProjetoRepository = new TipoProjetoRepository();
            _colaboradorRepository = new ColaboradorRepository();
            _equipeColaboradoresRepository = new EquipeColaboradoresRepository();
            _equipeRepository = new EquipeRepository();
            _colaboradorSkillsRepository = new ColaboradorTechSkillsRepository();
            _algoritimoAnaliseMethods = new AlgoritimoAnaliseMethods();
            _geminiService = new GeminiServiceRepository();
            _techSkillRepository = new TechSkillRepository();
            _analiseRepository = new AnaliseProjetoRepository();
            _riscoRepository = new RiscoRepository();
            _context = new ZenithContext();
        }




        [HttpGet("ListarTodos")]
        public IActionResult GetAll()
        {
            try
            {
                List<Projeto> projetos = _projetoRepository.ListarTodos();

                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projeto.IdUsuario);

                        if (usuarioBuscado == null)
                        {
                            return NotFound("Usuário não encontrado");
                        }

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("BuscarPeloId/{idProjeto}")]
        public IActionResult GetById(Guid idProjeto)
        {
            try
            {
                Projeto projetoBuscado = _projetoRepository.BuscarPorId(idProjeto);

                if (projetoBuscado == null)
                {
                    return NotFound("Projeto não encontrado");
                }

                VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                {
                    Id = projetoBuscado.Id,
                    Nome = projetoBuscado.Nome,
                    Descricao = projetoBuscado.Descricao,
                    DataInicio = projetoBuscado.DataInicio,
                    DataFinal = projetoBuscado.DataFinal,
                    NivelProjeto = (projetoBuscado.NivelProjeto == 1) ? "Baixo" : ((projetoBuscado.NivelProjeto == 2) ? "Médio" : ((projetoBuscado.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                    NivelAnalise = projetoBuscado.NivelAnalise,
                    TipoProjeto = projetoBuscado.TipoProjeto!.Tipo,
                    Funcionalidades = new List<Funcionalidade>(),
                    Tecnologias = new List<Tecnologia>()
                };

                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projetoBuscado.IdUsuario);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                {
                    IdUsuario = usuarioBuscado.Id,
                    Nome = usuarioBuscado.Nome,
                    Email = usuarioBuscado.Email,
                    Foto = usuarioBuscado.Foto,
                    Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                    Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                    Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                };

                List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                if (listaFuncionalidades.Count > 0)
                {
                    foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                    {
                        projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                    }
                }

                List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                if (listaTecnologias.Count > 0)
                {
                    foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                    {
                        projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                    }
                }

                //Adiciona a equipe do projeto

                Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projetoBuscado.Id);

                if (equipeBuscada != null)
                {
                    VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                    {
                        Id = equipeBuscada.Id,
                        QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                        Colaboradores = new List<ListagemUsuarioViewModel>()
                    };

                    List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                    List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                    if (listaColaboradores.Any())
                    {

                        foreach (Colaborador colaborador in listaColaboradores)
                        {
                            ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                            {
                                Nome = colaborador.Usuario.Nome,
                                Email = colaborador.Usuario.Email,
                                Foto = colaborador.Usuario.Foto,
                                IdUsuario = colaborador.IdUsuario,
                                IdColaborador = colaborador.Id,
                                Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                Perfil = "Colaborador",
                                Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                            };

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

                            colaboradoresEquipe.Add(usuarioViewModel);
                        }
                    }
                    equipeViewModel.Colaboradores = colaboradoresEquipe;

                    projetoViewModel.Equipe = equipeViewModel;
                }

                return Ok(projetoViewModel);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosPeloUsuario/{idUsuario}")]
        public IActionResult GetByUserId(Guid idUsuario)
        {
            try
            {
                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(idUsuario);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                List<Projeto> projetos = _projetoRepository.ListarPorUsuario(idUsuario);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo
                        };

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        List<Funcionalidade> funcionalidades = new List<Funcionalidade>();

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        projetoViewModel.Funcionalidades = funcionalidades;

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);


                        List<Tecnologia> tecnologias = new List<Tecnologia>();

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        projetoViewModel.Tecnologias = tecnologias;

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpPost("CadastrarProjeto/{idUsuario}")]
        public async Task<IActionResult> Post(Guid idUsuario, [FromBody] CadastrarProjetoViewModel infoNovoProjeto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    TipoProjeto tipoProjetoBuscado = _tipoProjetoRepository.BuscarPorNome(infoNovoProjeto.TipoProjeto);

                    if (tipoProjetoBuscado == null)
                    {
                        TipoProjeto novoTipoProjeto = new TipoProjeto()
                        {
                            Tipo = infoNovoProjeto.TipoProjeto,
                        };

                        _tipoProjetoRepository.Cadastrar(novoTipoProjeto);

                        tipoProjetoBuscado = novoTipoProjeto;
                    }

                    Projeto novoProjeto = new Projeto()
                    {
                        Nome = infoNovoProjeto.Nome,
                        Descricao = infoNovoProjeto.Descricao,
                        DataInicio = infoNovoProjeto.DataInicio,
                        DataFinal = infoNovoProjeto.DataFinal,
                        IdTipoProjeto = tipoProjetoBuscado.Id,
                        IdUsuario = idUsuario,
                        NivelProjeto = 0,
                        NivelAnalise = 0
                    };


                    // Cadastrar Projeto
                    _projetoRepository.Cadastrar(novoProjeto);

                    // Cadastrando a Análise do Projeto

                    // Pegar a quantidade de colaboradores por área e senioridade
                    List<Usuario> usuariosTecnicos = _usuarioRepository.ListarPorAreaCargo("Tecnica");
                    List<Usuario> usuariosFuncionais = _usuarioRepository.ListarPorAreaCargo("Funcional");
                    List<Usuario> usuariosAmbientais = _usuarioRepository.ListarPorAreaCargo("Ambiente");

                    // Montar a matriz com a quantidade de colaboradores
                    int[] arrayTecnica = new int[4] { 0, 0, 0, 0 };
                    int[] arrayFuncional = new int[4] { 0, 0, 0, 0 };
                    int[] arrayAmbiente = new int[4] { 0, 0, 0, 0 };

                    if (usuariosTecnicos.Count > 0)
                    {
                        foreach (Usuario usuario in usuariosTecnicos)
                        {
                            switch (usuario.NivelSenioridade)
                            {
                                case 1:
                                    arrayTecnica[3]++;
                                    break;

                                case 2:
                                    arrayTecnica[2]++;
                                    break;

                                case 3:
                                    arrayTecnica[1]++;
                                    break;

                                case 4:
                                    arrayTecnica[0]++;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    if (usuariosFuncionais.Count > 0)
                    {
                        foreach (Usuario usuario in usuariosTecnicos)
                        {
                            switch (usuario.NivelSenioridade)
                            {
                                case 1:
                                    arrayFuncional[3]++;
                                    break;

                                case 2:
                                    arrayFuncional[2]++;
                                    break;

                                case 3:
                                    arrayFuncional[1]++;
                                    break;

                                case 4:
                                    arrayFuncional[0]++;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    if (usuariosAmbientais.Count > 0)
                    {
                        foreach (Usuario usuario in usuariosTecnicos)
                        {
                            switch (usuario.NivelSenioridade)
                            {
                                case 1:
                                    arrayAmbiente[3]++;
                                    break;

                                case 2:
                                    arrayAmbiente[2]++;
                                    break;

                                case 3:
                                    arrayAmbiente[1]++;
                                    break;

                                case 4:
                                    arrayAmbiente[0]++;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    int[,] matrizColaboradores = new int[3, 4];

                    for (int i = 0; i < matrizColaboradores.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrizColaboradores.GetLength(1); j++)
                        {
                            if (i == 0)
                            {
                                matrizColaboradores[i, j] = arrayTecnica[j];
                            }
                            else if (i == 1)
                            {
                                matrizColaboradores[i, j] = arrayFuncional[j];
                            }
                            else
                            {
                                matrizColaboradores[i, j] = arrayAmbiente[j];
                            }
                        }
                    }


                    // Realizar Análise do Projeto

                    InfoProjectSettings informacoesProjeto = new InfoProjectSettings()
                    {
                        Nome = infoNovoProjeto.Nome,
                        Descricao = infoNovoProjeto.Descricao,
                        Tipo = infoNovoProjeto.TipoProjeto,
                        Funcionalidades = infoNovoProjeto.Funcionalidades,
                        Tecnologias = infoNovoProjeto.Tecnologias,
                        DataInicial = infoNovoProjeto.DataInicio?.ToString("dd/MM/yyyy"),
                        DataFinal = infoNovoProjeto.DataFinal?.ToString("dd/MM/yyyy")
                    };

                    AnaliseProjetoViewModel analiseProjeto = await _algoritimoAnaliseMethods.AnalisarRiscosProjeto(informacoesProjeto);

                    // Avaliar as áreas do projeto

                    int[,] rangeComplexidadeProjeto = await _geminiService.AvaliarRangeComplexidadeProjeto(informacoesProjeto, analiseProjeto.AnaliseGeral);


                    // Definir a Composição da Equipe

                    float[] composicaoEquipeIdeal = _algoritimoAnaliseMethods.DefinirComposicaoDaEquipe(matrizColaboradores, rangeComplexidadeProjeto);

                    AnaliseProjeto novaAnalise = new AnaliseProjeto()
                    {
                        DescricaoGeral = analiseProjeto.AnaliseGeral,
                        IdProjeto = novoProjeto.Id,
                        GestoresIdeais = composicaoEquipeIdeal[0],
                        SenioresIdeais = composicaoEquipeIdeal[1],
                        PlenosIdeais = composicaoEquipeIdeal[2],
                        JuniorsIdeais = composicaoEquipeIdeal[3]
                    };

                    _analiseRepository.CadastrarAnalise(novaAnalise);

                    foreach (RiskSettings risco in analiseProjeto.RiscosAnalise)
                    {
                        Risco novoRisco = new Risco()
                        {
                            IdAnaliseProjeto = novaAnalise.Id,
                            DescricaoRisco = risco.Descricao,
                            AreaRisco = risco.Area,
                            ImpactoRisco = risco.Impacto,
                            ProbabilidadeRisco = risco.Probabilidade,
                            NivelRisco = risco.Nivel
                        };

                        TechSkill skillBuscada = _techSkillRepository.BuscarPorNome(risco.TechSkill);

                        if (skillBuscada == null)
                        {
                            skillBuscada = new TechSkill()
                            {
                                Skill = risco.TechSkill
                            };

                            _techSkillRepository.Cadastrar(skillBuscada);
                        }

                        novoRisco.IdTechSkill = skillBuscada.Id;

                        _riscoRepository.Cadastrar(novoRisco);
                    }

                    // Atualizar o nível do projeto

                    int nivelProjeto = await _geminiService.RealizarAnaliseComplexidadeProjeto(informacoesProjeto, analiseProjeto.AnaliseGeral, composicaoEquipeIdeal);

                    _projetoRepository.AtualizarNivelAnalise(novoProjeto.Id, nivelProjeto);

                    _projetoRepository.AtualizarNivel(novoProjeto.Id, nivelProjeto);

                    if (infoNovoProjeto.Funcionalidades != null)
                    {
                        for (int i = 0; i < infoNovoProjeto.Funcionalidades.Length; i++)
                        {
                            Funcionalidade funcionalidadeBuscada = _funcionalidadeRepository.BuscarPorNome(infoNovoProjeto.Funcionalidades[i]);

                            if (funcionalidadeBuscada == null)
                            {
                                funcionalidadeBuscada = new Funcionalidade()
                                {
                                    Descricao = infoNovoProjeto.Funcionalidades[i]
                                };

                                _funcionalidadeRepository.Cadastrar(funcionalidadeBuscada);
                            }

                            FuncionalidadesProjeto novaFuncionalidadeProjeto = new FuncionalidadesProjeto()
                            {
                                IdFuncionalidade = funcionalidadeBuscada.Id,
                                IdProjeto = novoProjeto.Id
                            };

                            _funcionalidadesProjetoRepository.Cadastrar(novaFuncionalidadeProjeto);
                        }

                    }

                    if (infoNovoProjeto.Tecnologias != null)
                    {
                        for (int i = 0; i < infoNovoProjeto.Tecnologias.Length; i++)
                        {
                            Tecnologia tecnologiaBuscada = _tecnologiaRepository.BuscarPorNome(infoNovoProjeto.Tecnologias[i]);

                            if (tecnologiaBuscada == null)
                            {
                                tecnologiaBuscada = new Tecnologia()
                                {
                                    NomeTecnologia = infoNovoProjeto.Tecnologias[i]
                                };

                                _tecnologiaRepository.Cadastrar(tecnologiaBuscada);
                            }

                            TecnologiasProjeto novaTecnologiaProjeto = new TecnologiasProjeto()
                            {
                                IdTecnologia = tecnologiaBuscada.Id,
                                IdProjeto = novoProjeto.Id
                            };

                            _tecnologiasProjetoRepository.Cadastrar(novaTecnologiaProjeto);
                        }

                    }

                    // Confirma que tudo deu certo
                    transaction.Commit();

                    return Ok(novoProjeto.Id);
                }
                catch (Exception erro)
                {
                    // Desfaz as alterações caso algo dê errado
                    transaction.Rollback();

                    return BadRequest(erro.Message);
                }
            }
        }


        [HttpDelete("DeletarProjeto/{idProjeto}")]
        public IActionResult Delete(Guid idProjeto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Projeto projetoBuscado = _projetoRepository.BuscarPorId(idProjeto);

                    if (projetoBuscado == null)
                    {
                        return NotFound("Projeto não encontrado");
                    }

                    List<FuncionalidadesProjeto> listaFuncionalidadesProjeto = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                    if (listaFuncionalidadesProjeto.Count > 0)
                    {
                        foreach (FuncionalidadesProjeto funcionalidadeProjeto in listaFuncionalidadesProjeto)
                        {
                            _funcionalidadesProjetoRepository.Deletar(funcionalidadeProjeto.Id);
                        }
                    }

                    List<TecnologiasProjeto> listaTecnologiasProjeto = _tecnologiasProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                    if (listaTecnologiasProjeto.Count > 0)
                    {
                        foreach (TecnologiasProjeto tecnologiaProjeto in listaTecnologiasProjeto)
                        {
                            _tecnologiasProjetoRepository.Deletar(tecnologiaProjeto.Id);
                        }
                    }

                    //Trazer a equipe que faz parte do projeto
                    Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projetoBuscado.Id);

                    if (equipeBuscada != null)
                    {
                        //Trazer os registros da tabela intermediária que contenham o id dessa equipe
                        List<EquipeColaboradores> equipeColaboradores = _equipeColaboradoresRepository.ListarPelaEquipe(equipeBuscada.Id);

                        if (equipeColaboradores.Count > 0)
                        {
                            foreach (EquipeColaboradores equipeColaborador in equipeColaboradores)
                            {
                                //Deletar todos os registros
                                _equipeColaboradoresRepository.Deletar(equipeColaborador.Id);
                            }
                        }

                        //Deletar a Equipe
                        _equipeRepository.Deletar(equipeBuscada.Id);
                    }

                    // Deletar a análise do projeto
                    AnaliseProjeto analiseBucada = _analiseRepository.BuscarPeloIdProjeto(projetoBuscado.Id);

                    if (analiseBucada != null)
                    {
                        // Deleta a análise e os riscos
                        _analiseRepository.Deletar(analiseBucada.Id);
                    }


                    // Deleta o projeto
                    _projetoRepository.Deletar(projetoBuscado.Id);

                    transaction.Commit();

                    return StatusCode(200, "Projeto deletado com sucesso");
                }
                catch (Exception erro)
                {
                    transaction.Rollback();

                    return BadRequest(erro.Message);
                }
            }
        }


        [HttpPut("AtualizarProjeto/{idProjeto}")]
        public async Task<IActionResult> Put(Guid idProjeto, CadastrarProjetoViewModel infoProjetoAtualizado)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Projeto projetoBuscado = _projetoRepository.BuscarPorId(idProjeto);

                    if (projetoBuscado == null)
                    {
                        return NotFound("Projeto não encontrado");
                    }

                    Projeto projetoAtualizado = new Projeto()
                    {
                        DataFinal = infoProjetoAtualizado.DataFinal,
                        DataInicio = infoProjetoAtualizado.DataInicio,
                        Descricao = infoProjetoAtualizado.Descricao,
                        Nome = infoProjetoAtualizado.Nome,
                    };

                    if (infoProjetoAtualizado.TipoProjeto != null)
                    {
                        TipoProjeto tipoProjetoBuscado = _tipoProjetoRepository.BuscarPorNome(infoProjetoAtualizado.TipoProjeto!);

                        if (tipoProjetoBuscado == null && infoProjetoAtualizado.TipoProjeto != null)
                        {
                            TipoProjeto novoTipoProjeto = new TipoProjeto()
                            {
                                Tipo = infoProjetoAtualizado.TipoProjeto
                            };

                            _tipoProjetoRepository.Cadastrar(novoTipoProjeto);

                            tipoProjetoBuscado = novoTipoProjeto;
                        }

                        projetoAtualizado.IdTipoProjeto = infoProjetoAtualizado.TipoProjeto != null ? tipoProjetoBuscado.Id : projetoBuscado.IdTipoProjeto;
                    }

                    _projetoRepository.Atualizar(projetoBuscado.Id, projetoAtualizado);


                    //Atualiza as tabelas intermediárias

                    //FuncionalidadesProjeto
                    List<FuncionalidadesProjeto> listaFuncionalidadesProjeto = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                    if (listaFuncionalidadesProjeto.Count > 0)
                    {
                        foreach (FuncionalidadesProjeto funcionalidadeProjeto in listaFuncionalidadesProjeto)
                        {
                            if (infoProjetoAtualizado.Funcionalidades == null || infoProjetoAtualizado.Funcionalidades.Length == 0)
                            {
                                _funcionalidadesProjetoRepository.Deletar(funcionalidadeProjeto.Id);
                            }
                            else
                            {
                                Funcionalidade funcionalidade = _funcionalidadeRepository.BuscarPorId(funcionalidadeProjeto.IdFuncionalidade);

                                bool manterFuncionalidade = false;

                                foreach (string novaFuncionalidade in infoProjetoAtualizado.Funcionalidades!)
                                {
                                    if (novaFuncionalidade == funcionalidade.Descricao)
                                    {
                                        manterFuncionalidade = true;
                                    }
                                }

                                if (!manterFuncionalidade)
                                {
                                    //Deletar registros de skills
                                    _funcionalidadesProjetoRepository.Deletar(funcionalidadeProjeto.Id);
                                }
                            }
                        }

                    }

                    if (infoProjetoAtualizado.Funcionalidades != null)
                    {
                        foreach (string novaFuncionalidade in infoProjetoAtualizado.Funcionalidades)
                        {
                            bool manterFuncionalidade = false;

                            foreach (FuncionalidadesProjeto funcionalidadeProjeto in listaFuncionalidadesProjeto)
                            {
                                Funcionalidade funcionalidade = _funcionalidadeRepository.BuscarPorId(funcionalidadeProjeto.IdFuncionalidade);

                                if (novaFuncionalidade == funcionalidade.Descricao)
                                {
                                    manterFuncionalidade = true;
                                }
                            }

                            if (!manterFuncionalidade)
                            {
                                //Cadastrar registros de skills
                                Funcionalidade funcionalidadeBuscada = _funcionalidadeRepository.BuscarPorNome(novaFuncionalidade);

                                if (funcionalidadeBuscada == null)
                                {
                                    Funcionalidade novoRegistroFuncionalidade = new Funcionalidade()
                                    {
                                        Descricao = novaFuncionalidade
                                    };

                                    _funcionalidadeRepository.Cadastrar(novoRegistroFuncionalidade);

                                    funcionalidadeBuscada = novoRegistroFuncionalidade;
                                }

                                FuncionalidadesProjeto novoRegistroFuncionalidadeProjeto = new FuncionalidadesProjeto()
                                {
                                    IdProjeto = projetoBuscado.Id,
                                    IdFuncionalidade = funcionalidadeBuscada.Id
                                };

                                _funcionalidadesProjetoRepository.Cadastrar(novoRegistroFuncionalidadeProjeto);
                            }
                        }
                    }

                    //===========================================================================================


                    //TecnologiasProjeto
                    List<TecnologiasProjeto> listaTecnologiasProjeto = _tecnologiasProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                    if (listaTecnologiasProjeto.Count > 0)
                    {
                        foreach (TecnologiasProjeto tecnologiaProjeto in listaTecnologiasProjeto)
                        {
                            if (infoProjetoAtualizado.Tecnologias == null || infoProjetoAtualizado.Tecnologias.Length == 0)
                            {
                                _tecnologiasProjetoRepository.Deletar(tecnologiaProjeto.Id);
                            }
                            else
                            {
                                Tecnologia tecnologia = _tecnologiaRepository.BuscarPorId(tecnologiaProjeto.IdTecnologia);

                                bool manterTecnologia = false;

                                foreach (string novaTecnologia in infoProjetoAtualizado.Tecnologias!)
                                {
                                    if (novaTecnologia == tecnologia.NomeTecnologia)
                                    {
                                        manterTecnologia = true;
                                    }
                                }

                                if (!manterTecnologia)
                                {
                                    //Deletar registros de skills
                                    _tecnologiasProjetoRepository.Deletar(tecnologiaProjeto.Id);
                                }
                            }
                        }

                    }

                    if (infoProjetoAtualizado.Tecnologias != null)
                    {
                        foreach (string novaTecnologia in infoProjetoAtualizado.Tecnologias)
                        {
                            bool manterTecnologia = false;

                            foreach (TecnologiasProjeto tecnologiaProjeto in listaTecnologiasProjeto)
                            {
                                Tecnologia tecnologia = _tecnologiaRepository.BuscarPorId(tecnologiaProjeto.IdTecnologia);

                                if (novaTecnologia == tecnologia.NomeTecnologia)
                                {
                                    manterTecnologia = true;
                                }
                            }

                            if (!manterTecnologia)
                            {
                                //Cadastrar registros de skills
                                Tecnologia tecnologiaBuscada = _tecnologiaRepository.BuscarPorNome(novaTecnologia);

                                if (tecnologiaBuscada == null)
                                {
                                    Tecnologia novoRegistroTecnologia = new Tecnologia()
                                    {
                                        NomeTecnologia = novaTecnologia
                                    };

                                    _tecnologiaRepository.Cadastrar(novoRegistroTecnologia);

                                    tecnologiaBuscada = novoRegistroTecnologia;
                                }

                                TecnologiasProjeto novoRegistroTecnologiaProjeto = new TecnologiasProjeto()
                                {
                                    IdProjeto = projetoBuscado.Id,
                                    IdTecnologia = tecnologiaBuscada.Id
                                };

                                _tecnologiasProjetoRepository.Cadastrar(novoRegistroTecnologiaProjeto);
                            }
                        }
                    }

                    // Atualizando a Análise do Projeto

                    // Pegar a quantidade de colaboradores por área e senioridade
                    List<Usuario> usuariosTecnicos = _usuarioRepository.ListarPorAreaCargo("Tecnica");
                    List<Usuario> usuariosFuncionais = _usuarioRepository.ListarPorAreaCargo("Funcional");
                    List<Usuario> usuariosAmbientais = _usuarioRepository.ListarPorAreaCargo("Ambiente");

                    // Montar a matriz com a quantidade de colaboradores
                    int[] arrayTecnica = new int[4] { 0, 0, 0, 0 };
                    int[] arrayFuncional = new int[4] { 0, 0, 0, 0 };
                    int[] arrayAmbiente = new int[4] { 0, 0, 0, 0 };

                    if (usuariosTecnicos.Count > 0)
                    {
                        foreach (Usuario usuario in usuariosTecnicos)
                        {
                            switch (usuario.NivelSenioridade)
                            {
                                case 1:
                                    arrayTecnica[3]++;
                                    break;

                                case 2:
                                    arrayTecnica[2]++;
                                    break;

                                case 3:
                                    arrayTecnica[1]++;
                                    break;

                                case 4:
                                    arrayTecnica[0]++;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    if (usuariosFuncionais.Count > 0)
                    {
                        foreach (Usuario usuario in usuariosTecnicos)
                        {
                            switch (usuario.NivelSenioridade)
                            {
                                case 1:
                                    arrayFuncional[3]++;
                                    break;

                                case 2:
                                    arrayFuncional[2]++;
                                    break;

                                case 3:
                                    arrayFuncional[1]++;
                                    break;

                                case 4:
                                    arrayFuncional[0]++;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    if (usuariosAmbientais.Count > 0)
                    {
                        foreach (Usuario usuario in usuariosTecnicos)
                        {
                            switch (usuario.NivelSenioridade)
                            {
                                case 1:
                                    arrayAmbiente[3]++;
                                    break;

                                case 2:
                                    arrayAmbiente[2]++;
                                    break;

                                case 3:
                                    arrayAmbiente[1]++;
                                    break;

                                case 4:
                                    arrayAmbiente[0]++;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    int[,] matrizColaboradores = new int[3, 4];

                    for (int i = 0; i < matrizColaboradores.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrizColaboradores.GetLength(1); j++)
                        {
                            if (i == 0)
                            {
                                matrizColaboradores[i, j] = arrayTecnica[j];
                            }
                            else if (i == 1)
                            {
                                matrizColaboradores[i, j] = arrayFuncional[j];
                            }
                            else
                            {
                                matrizColaboradores[i, j] = arrayAmbiente[j];
                            }
                        }
                    }



                    // Atualizar Análise do Projeto

                    AnaliseProjeto analiseBuscada = _analiseRepository.BuscarPeloIdProjeto(projetoBuscado.Id);

                    InfoProjectSettings informacoesProjeto = new InfoProjectSettings()
                    {
                        Nome = projetoAtualizado.Nome,
                        Descricao = projetoAtualizado.Descricao,
                        Tipo = infoProjetoAtualizado.TipoProjeto != null ? infoProjetoAtualizado.TipoProjeto : projetoBuscado.TipoProjeto.Tipo,
                        Funcionalidades = infoProjetoAtualizado.Funcionalidades,
                        Tecnologias = infoProjetoAtualizado.Tecnologias,
                        DataInicial = projetoAtualizado.DataInicio?.ToString("dd/MM/yyyy"),
                        DataFinal = projetoAtualizado.DataFinal?.ToString("dd/MM/yyyy")
                    };

                    AnaliseProjetoViewModel analiseProjeto = await _algoritimoAnaliseMethods.AnalisarRiscosProjeto(informacoesProjeto);

                    // Avaliar as áreas do projeto

                    int[,] rangeComplexidadeProjeto = await _geminiService.AvaliarRangeComplexidadeProjeto(informacoesProjeto, analiseProjeto.AnaliseGeral);


                    // Definir a Composição da Equipe

                    float[] composicaoEquipeIdeal = _algoritimoAnaliseMethods.DefinirComposicaoDaEquipe(matrizColaboradores, rangeComplexidadeProjeto);

                    AnaliseProjeto novaAnalise = new AnaliseProjeto()
                    {
                        DescricaoGeral = analiseProjeto.AnaliseGeral,
                        IdProjeto = projetoBuscado.Id,
                        GestoresIdeais = composicaoEquipeIdeal[0],
                        SenioresIdeais = composicaoEquipeIdeal[1],
                        PlenosIdeais = composicaoEquipeIdeal[2],
                        JuniorsIdeais = composicaoEquipeIdeal[3]
                    };


                    // Deletar a analise e cadastrar uma nova

                    _analiseRepository.Deletar(analiseBuscada.Id);

                    _analiseRepository.CadastrarAnalise(novaAnalise);

                    // Cadastrar os novos riscos
                    foreach (RiskSettings risco in analiseProjeto.RiscosAnalise)
                    {
                        Risco novoRisco = new Risco()
                        {
                            IdAnaliseProjeto = novaAnalise.Id,
                            DescricaoRisco = risco.Descricao,
                            AreaRisco = risco.Area,
                            ImpactoRisco = risco.Impacto,
                            ProbabilidadeRisco = risco.Probabilidade,
                            NivelRisco = risco.Nivel
                        };

                        TechSkill skillBuscada = _techSkillRepository.BuscarPorNome(risco.TechSkill);

                        if (skillBuscada == null)
                        {
                            skillBuscada = new TechSkill()
                            {
                                Skill = risco.TechSkill
                            };

                            _techSkillRepository.Cadastrar(skillBuscada);
                        }

                        novoRisco.IdTechSkill = skillBuscada.Id;

                        _riscoRepository.Cadastrar(novoRisco);
                    }

                    // Atualizar o nível do projeto

                    int nivelProjeto = await _geminiService.RealizarAnaliseComplexidadeProjeto(informacoesProjeto, analiseProjeto.AnaliseGeral, composicaoEquipeIdeal);

                    _projetoRepository.AtualizarNivelAnalise(projetoBuscado.Id, nivelProjeto);

                    _projetoRepository.AtualizarNivel(projetoBuscado.Id, nivelProjeto);


                    transaction.Commit();

                    return Ok("Projeto atualizado com sucesso");
                }
                catch (Exception erro)
                {
                    transaction.Rollback();

                    return BadRequest(erro.Message);
                }
            }
        }


        [HttpPut("AtualizarNivelProjeto/{idProjeto}")]
        public IActionResult AtualizarNivel(Guid idProjeto, int novoNivel)
        {
            try
            {
                Projeto projetoBuscado = _projetoRepository.BuscarPorId(idProjeto);

                if (projetoBuscado == null)
                {
                    return NotFound("Projeto não encontrado");
                }

                _projetoRepository.AtualizarNivel(idProjeto, novoNivel);

                return Ok("Nivel atualizado com sucesso");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }


        [HttpGet("ListarProjetosPeloTipo")]
        public IActionResult ListarProjetosPeloTipo(string tipoProjeto)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                List<Projeto> projetos = _projetoRepository.ListarPorTipo(tipoProjeto);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projeto.IdUsuario);

                        if (usuarioBuscado == null)
                        {
                            return NotFound("Usuário não encontrado");
                        }

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosPeloTipo/{idUsuario}")]
        public IActionResult ListarProjetosPeloNivelGerente(Guid idUsuario, string tipoProjeto)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                List<Projeto> projetos = _projetoRepository.ListarPorTipo(tipoProjeto);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(idUsuario);

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }


        [HttpGet("ListarProjetosEmAndamento")]
        public IActionResult ListarProjetosEmAndamento()
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                List<Projeto> projetos = _projetoRepository.ListarProjetosEmAndamento();

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projeto.IdUsuario);

                        if (usuarioBuscado == null)
                        {
                            return NotFound("Usuário não encontrado");
                        }

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        List<Risco> riscos = new List<Risco>();

                        AnaliseProjeto analiseBucada = _analiseRepository.BuscarPeloIdProjeto(projeto.Id);

                        if (analiseBucada != null)
                        {
                            List<Risco> riscosAnalise = _riscoRepository.ListarPelaAnalise(analiseBucada.Id);

                            foreach (Risco risco in riscosAnalise)
                            {
                                riscos.Add(risco);
                            }
                        }

                        projetoViewModel.Riscos = riscos;

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosQueNaoEstaoEmAndamento")]
        public IActionResult ListarProjetosQueNaoEstaoEmAndamento()
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                List<Projeto> projetos = _projetoRepository.ListarProjetosQueNaoEstaoEmAndamento();

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projeto.IdUsuario);

                        if (usuarioBuscado == null)
                        {
                            return NotFound("Usuário não encontrado");
                        }

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosConcluidos")]
        public IActionResult ListarProjetosConcluidos()
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                List<Projeto> projetos = _projetoRepository.ListarProjetosConcluidos();

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projeto.IdUsuario);

                        if (usuarioBuscado == null)
                        {
                            return NotFound("Usuário não encontrado");
                        }

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosFuturos")]
        public IActionResult ListarProjetosFuturos()
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();


                List<Projeto> projetos = _projetoRepository.ListarProjetosFuturos();

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projeto.IdUsuario);

                        if (usuarioBuscado == null)
                        {
                            return NotFound("Usuário não encontrado");
                        }

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }



        [HttpGet("ListarProjetosEmAndamento/{idUsuario}")]
        public IActionResult ListarProjetosEmAndamentoGerente(Guid idUsuario)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(idUsuario);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                List<Projeto> projetos = _projetoRepository.ListarProjetosEmAndamento(idUsuario);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        List<Risco> riscos = new List<Risco>();

                        AnaliseProjeto analiseBucada = _analiseRepository.BuscarPeloIdProjeto(projeto.Id);

                        if (analiseBucada != null)
                        {
                            List<Risco> riscosAnalise = _riscoRepository.ListarPelaAnalise(analiseBucada.Id);

                            foreach (Risco risco in riscosAnalise)
                            {
                                riscos.Add(risco);
                            }
                        }

                        projetoViewModel.Riscos = riscos;

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }



                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }


        [HttpGet("ListarProjetosConcluidos/{idUsuario}")]
        public IActionResult ListarProjetosConcluidosGerente(Guid idUsuario)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(idUsuario);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                List<Projeto> projetos = _projetoRepository.ListarProjetosConcluidos(idUsuario);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosFuturos/{idUsuario}")]
        public IActionResult ListarProjetosFuturosGerente(Guid idUsuario)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(idUsuario);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                List<Projeto> projetos = _projetoRepository.ListarProjetosFuturos(idUsuario);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosQueNaoEstaoEmAndamento/{idUsuario}")]
        public IActionResult ListarProjetosQueNaoEstaoEmAndamentoGerente(Guid idUsuario)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                List<Projeto> projetos = _projetoRepository.ListarProjetosQueNaoEstaoEmAndamento(idUsuario);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projeto.IdUsuario);

                        if (usuarioBuscado == null)
                        {
                            return NotFound("Usuário não encontrado");
                        }

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        //Adiciona a equipe do projeto

                        Equipe equipeBuscada = _equipeRepository.BuscarPorIdProjeto(projeto.Id);

                        if (equipeBuscada != null)
                        {
                            VisualizarEquipeViewModel equipeViewModel = new VisualizarEquipeViewModel()
                            {
                                Id = equipeBuscada.Id,
                                QtdIntegrantes = equipeBuscada.QtdIntegrantes,
                                Colaboradores = new List<ListagemUsuarioViewModel>()
                            };

                            List<Colaborador> listaColaboradores = _equipeRepository.ListarColaboradoresPorEquipe(equipeBuscada.Id);

                            List<ListagemUsuarioViewModel> colaboradoresEquipe = new List<ListagemUsuarioViewModel>();

                            if (listaColaboradores.Any())
                            {

                                foreach (Colaborador colaborador in listaColaboradores)
                                {
                                    ListagemUsuarioViewModel usuarioViewModel = new ListagemUsuarioViewModel()
                                    {
                                        Nome = colaborador.Usuario.Nome,
                                        Email = colaborador.Usuario.Email,
                                        Foto = colaborador.Usuario.Foto,
                                        IdUsuario = colaborador.IdUsuario,
                                        IdColaborador = colaborador.Id,
                                        Cargo = colaborador.Usuario.CargoUsuario.Cargo,
                                        Perfil = "Colaborador",
                                        Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))

                                    };

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

                                    colaboradoresEquipe.Add(usuarioViewModel);
                                }
                            }
                            equipeViewModel.Colaboradores = colaboradoresEquipe;

                            projetoViewModel.Equipe = equipeViewModel;
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }



        [HttpGet("ListarProjetosPeloNivel")]
        public IActionResult ListarProjetosPeloNivel(int nivelProjeto)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                if (nivelProjeto < 0 || nivelProjeto > 3)
                {
                    return NotFound("Não existe projetos com o nível especificado");
                }

                List<Projeto> projetos = _projetoRepository.ListaPorNivel(nivelProjeto);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projeto.IdUsuario);

                        if (usuarioBuscado == null)
                        {
                            return NotFound("Usuário não encontrado");
                        }

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosPeloNivel/{idUsuario}")]
        public IActionResult ListarProjetosPeloNivelGerente(Guid idUsuario, int nivelProjeto)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(idUsuario);

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                if (nivelProjeto < 0 || nivelProjeto > 3)
                {
                    return NotFound("Não existe projetos com o nível especificado");
                }

                List<Projeto> projetos = _projetoRepository.ListaPorNivel(nivelProjeto, usuarioBuscado.Id);

                if (projetos.Count > 0)
                {
                    foreach (Projeto projeto in projetos)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projeto.Id,
                            Nome = projeto.Nome,
                            Descricao = projeto.Descricao,
                            DataInicio = projeto.DataInicio,
                            DataFinal = projeto.DataFinal,
                            NivelProjeto = (projeto.NivelProjeto == 1) ? "Baixo" : ((projeto.NivelProjeto == 2) ? "Médio" : ((projeto.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projeto.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projeto.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        listaDeProjetos.Add(projetoViewModel);

                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpGet("ListarProjetosPeloNivelColaborador/{idColaborador}")]
        public IActionResult ListarProjetosPeloNivelColaborador(Guid idColaborador, int nivelProjeto)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                Colaborador colaboradorBuscado = _colaboradorRepository.BuscarPeloId(idColaborador);

                if (colaboradorBuscado == null)
                {
                    return NotFound("Colaborador não encontrado");
                }

                List<Projeto> listaDeProjetosPorColaborador = _projetoRepository.ListaPorNivelColaborador(nivelProjeto, colaboradorBuscado.Id);

                if (listaDeProjetosPorColaborador.Count > 0)
                {
                    foreach (Projeto projetoBuscado in listaDeProjetosPorColaborador)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projetoBuscado.Id,
                            Nome = projetoBuscado.Nome,
                            Descricao = projetoBuscado.Descricao,
                            DataInicio = projetoBuscado.DataInicio,
                            DataFinal = projetoBuscado.DataFinal,
                            NivelProjeto = (projetoBuscado.NivelProjeto == 1) ? "Baixo" : ((projetoBuscado.NivelProjeto == 2) ? "Médio" : ((projetoBuscado.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projetoBuscado.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projetoBuscado.IdUsuario);

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        listaDeProjetos.Add(projetoViewModel);
                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }



        [HttpGet("ListarPeloColaborador/{idColaborador}")]
        public IActionResult ListarPeloColaborador(Guid idColaborador)
        {
            try
            {
                List<VisualizarProjetoViewModel> listaDeProjetos = new List<VisualizarProjetoViewModel>();

                Colaborador colaboradorBuscado = _colaboradorRepository.BuscarPeloId(idColaborador);

                if (colaboradorBuscado == null)
                {
                    return NotFound("Colaborador não encontrado");
                }

                List<Projeto> listaDeProjetosPorColaborador = _projetoRepository.ListaPorColaborador(colaboradorBuscado.Id);

                if (listaDeProjetosPorColaborador.Count > 0)
                {
                    foreach (Projeto projetoBuscado in listaDeProjetosPorColaborador)
                    {
                        VisualizarProjetoViewModel projetoViewModel = new VisualizarProjetoViewModel()
                        {
                            Id = projetoBuscado.Id,
                            Nome = projetoBuscado.Nome,
                            Descricao = projetoBuscado.Descricao,
                            DataInicio = projetoBuscado.DataInicio,
                            DataFinal = projetoBuscado.DataFinal,
                            NivelProjeto = (projetoBuscado.NivelProjeto == 1) ? "Baixo" : ((projetoBuscado.NivelProjeto == 2) ? "Médio" : ((projetoBuscado.NivelProjeto == 3) ? "Alto" : "A ser analisado")),
                            TipoProjeto = projetoBuscado.TipoProjeto!.Tipo,
                            Funcionalidades = new List<Funcionalidade>(),
                            Tecnologias = new List<Tecnologia>()
                        };

                        Usuario usuarioBuscado = _usuarioRepository.BuscarPorId(projetoBuscado.IdUsuario);

                        projetoViewModel.Gerente = new ListagemUsuarioViewModel()
                        {
                            IdUsuario = usuarioBuscado.Id,
                            Nome = usuarioBuscado.Nome,
                            Email = usuarioBuscado.Email,
                            Foto = usuarioBuscado.Foto,
                            Cargo = usuarioBuscado.CargoUsuario!.Cargo,
                            Perfil = usuarioBuscado.CargoUsuario!.Cargo,
                            Senioridade = (usuarioBuscado.NivelSenioridade == 1) ? "Júnior" : ((usuarioBuscado.NivelSenioridade == 2) ? "Pleno" : ((usuarioBuscado.NivelSenioridade == 3) ? "Sênior" : "Gestor"))
                        };

                        List<FuncionalidadesProjeto> listaFuncionalidades = _funcionalidadesProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                        if (listaFuncionalidades.Count > 0)
                        {
                            foreach (FuncionalidadesProjeto funcionalidade in listaFuncionalidades)
                            {
                                projetoViewModel.Funcionalidades.Add(funcionalidade.Funcionalidade);
                            }
                        }

                        List<TecnologiasProjeto> listaTecnologias = _tecnologiasProjetoRepository.ListarPorIdProjeto(projetoBuscado.Id);

                        if (listaTecnologias.Count > 0)
                        {
                            foreach (TecnologiasProjeto tecnologia in listaTecnologias)
                            {
                                projetoViewModel.Tecnologias.Add(tecnologia.Tecnologia);
                            }
                        }

                        listaDeProjetos.Add(projetoViewModel);
                    }
                }

                return Ok(listaDeProjetos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
