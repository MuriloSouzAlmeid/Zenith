using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;
using ZenithWepAPI.Repositories;

namespace ZenithWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RiscoController : ControllerBase
    {
        private readonly IRiscoRepository _riscoRepository;
        private readonly IAnaliseProjetoRepository _analiseProjetoRepository;

        public RiscoController()
        {
            _riscoRepository = new RiscoRepository();
            _analiseProjetoRepository = new AnaliseProjetoRepository();
        }

        [HttpGet("ListarPelaAnalise/{idProjeto}")]
        public IActionResult GetByAnalise(Guid idProjeto)
        {
            try
            {
                List<Risco> riscos = new List<Risco>();

                AnaliseProjeto analiseBucada = _analiseProjetoRepository.BuscarPeloIdProjeto(idProjeto);

                if (analiseBucada != null)
                {
                    List<Risco> riscosAnalise = _riscoRepository.ListarPelaAnalise(analiseBucada.Id);

                    foreach (Risco risco in riscosAnalise)
                    {
                        riscos.Add(risco);
                    }
                }

                return Ok(riscos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpDelete("DeletarPelaAnalise/{idProjeto}")]
        public IActionResult DeleteByAnalise(Guid idProjeto)
        {
            try
            {
                AnaliseProjeto analiseBucada = _analiseProjetoRepository.BuscarPeloIdProjeto(idProjeto);

                if (analiseBucada != null)
                {
                    List<Risco> riscosAnalise = _riscoRepository.ListarPelaAnalise(analiseBucada.Id);

                    foreach (Risco risco in riscosAnalise)
                    {
                        // Deleta os riscos atrelados a esta análise
                        _riscoRepository.Deletar(risco.Id);
                    }
                }

                return Ok("Riscos Deletados");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
