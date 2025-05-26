using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZenithWepAPI.Utils.AlgoritmoAnalise;
using ZenithWepAPI.Utils.GeminiService;
using ZenithWepAPI.ViewModels;

namespace ZenithWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AlgoritmoAnaliseController : ControllerBase
    {
        private readonly AlgoritimoAnaliseMethods algoritimoMethods;

        public AlgoritmoAnaliseController()
        {
            algoritimoMethods = new AlgoritimoAnaliseMethods();
        }

        [HttpPost("ListarRiscosDaAnalise")]
        public async Task<IActionResult> ListarRiscosDaAnalise([FromBody] InfoProjectSettings informacoesProjeto)
        {
            try
            {
                AnaliseProjetoViewModel analiseProjeto = await algoritimoMethods.AnalisarRiscosProjeto(informacoesProjeto);

                return Ok(analiseProjeto);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }


    }
}
