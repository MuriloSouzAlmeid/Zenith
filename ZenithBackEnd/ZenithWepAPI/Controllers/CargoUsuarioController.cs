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
    public class CargoUsuarioController : ControllerBase
    {
        private readonly ICargoUsuarioRepository _cargoUsuarioRepository;

        public CargoUsuarioController()
        {
            _cargoUsuarioRepository = new CargoUsuarioRepositry();
        }

        [HttpGet("ListarTodos")]
        public ActionResult ListarTodos() {
            try
            {
                List<CargoUsuario> listaDeCargos = _cargoUsuarioRepository.ListarTodos();

                return Ok(listaDeCargos);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
