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
    public class TechSkillController : ControllerBase
    {
        private readonly ITechSkillRepository _techSkillRepository;

        public TechSkillController()
        {
            _techSkillRepository = new TechSkillRepository();
        }

        [HttpGet("ListarTodas")]
        public IActionResult Get()
        {
            try
            {
                List<TechSkill> listaDeTechSkills = _techSkillRepository.ListarTodas();

                return Ok(listaDeTechSkills);
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
