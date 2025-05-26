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
    public class AnaliseProjetoController : ControllerBase
    {
        private readonly IAnaliseProjetoRepository _analiseRepository;
        private readonly IProjetoRepository _projetoRepository;
        private readonly IFuncionalidadesProjetoRepository _funcionalidadesProjetoRepository;
        private readonly ITecnologiasProjetoRepository _tecnologiasProjetoRepository;
        private readonly IRiscoRepository _riscoRepository;

        public AnaliseProjetoController()
        {
            _analiseRepository = new AnaliseProjetoRepository();
            _projetoRepository = new ProjetoRepository();
            _funcionalidadesProjetoRepository = new FuncionalidadesProjetoRepository();
            _tecnologiasProjetoRepository = new TecnologiasProjetoRepository();
            _riscoRepository = new RiscoRepository();
        }

        [HttpGet("BuscarPeloIdProjeto/{idProjeto}")]
        public IActionResult GetByProjectId(Guid idProjeto)
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

                AnaliseProjeto analiseBuscada = _analiseRepository.BuscarPeloIdProjeto(projetoBuscado.Id);

                if(analiseBuscada == null)
                {
                    return NotFound("Não há análise para este projeto");
                }

                VisualizacaoAnaliseViewModel analiseViewModel = new VisualizacaoAnaliseViewModel() 
                {
                    Id = analiseBuscada.Id,
                    Descricao = analiseBuscada.DescricaoGeral,
                    ComposicaoEquipeIdeal = new float[4],
                    Projeto = projetoViewModel
                };

                List<VisualizarRiscoViewModel> riscosAnalise = new List<VisualizarRiscoViewModel>();

                foreach(Risco risco in analiseBuscada.Riscos)
                {
                    VisualizarRiscoViewModel riscoViewModel = new VisualizarRiscoViewModel()
                    {
                        Id= risco.Id,
                        Descricao = risco.DescricaoRisco,
                        TechSkill = risco.TechSkill.Skill,
                        Nivel = risco.NivelRisco,
                        Area = risco.AreaRisco,
                    };

                    riscosAnalise.Add(riscoViewModel);
                }

                analiseViewModel.Riscos = riscosAnalise;

                analiseViewModel.ComposicaoEquipeIdeal[0] = analiseBuscada.GestoresIdeais;
                analiseViewModel.ComposicaoEquipeIdeal[1] = analiseBuscada.SenioresIdeais;
                analiseViewModel.ComposicaoEquipeIdeal[2] = analiseBuscada.PlenosIdeais;
                analiseViewModel.ComposicaoEquipeIdeal[3] = analiseBuscada.JuniorsIdeais;

                return Ok(analiseViewModel);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpDelete("DeletarPeloProjeto/{idProjeto}")]
        public IActionResult DeleteByProjectId(Guid idProjeto)
        {
            try
            {
                AnaliseProjeto analiseBucada = _analiseRepository.BuscarPeloIdProjeto(idProjeto);

                if (analiseBucada != null)
                {
                    List<Risco> riscosAnalise = _riscoRepository.ListarPelaAnalise(analiseBucada.Id);

                    if (riscosAnalise.Count > 0)
                    {
                        foreach (Risco risco in riscosAnalise)
                        {
                            // Deleta os riscos atrelados a esta análise
                            _riscoRepository.Deletar(risco.Id);
                        }
                    }

                    // Deleta a análise
                    _analiseRepository.Deletar(analiseBucada.Id);
                }

                return Ok("Análise deletada");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
