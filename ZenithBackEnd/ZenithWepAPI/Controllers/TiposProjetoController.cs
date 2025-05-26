using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZenithWepAPI.Interfaces;
using ZenithWepAPI.Repositories;

namespace ZenithWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TiposProjetoController : ControllerBase
    {
        private readonly ITipoProjetoRepository _tipoRepository;

        public TiposProjetoController()
        {
            _tipoRepository = new TipoProjetoRepository();
        }

        [HttpGet("ListarTodos")]
        public IActionResult GetAll() 
        {
            try
            {
                return Ok(_tipoRepository.ListarTodas());
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
