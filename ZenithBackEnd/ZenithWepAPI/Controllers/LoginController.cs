using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Interfaces;
using ZenithWepAPI.Repositories;
using ZenithWepAPI.ViewModels;

namespace ZenithWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IColaboradorRepository _colaboradorRepository;

        public LoginController()
        {
            _usuarioRepository = new UsuarioRepository();
            _colaboradorRepository = new ColaboradorRepository();
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginViewModel infoLogin)
        {
            try
            {
                Usuario usuarioBuscado = _usuarioRepository.BuscarPorLogin(infoLogin.Email!, infoLogin.Senha!);

                if(usuarioBuscado == null)
                {
                    return NotFound("Email ou senha incorretos");
                }

                Colaborador colaboradorBuscado = _colaboradorRepository.BuscarPorIdUsuario(usuarioBuscado.Id);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, usuarioBuscado.Email!),
                    new Claim(JwtRegisteredClaimNames.Name, usuarioBuscado.Nome!),
                    new Claim(JwtRegisteredClaimNames.Jti, usuarioBuscado.Id.ToString()),
                    new Claim("photo", usuarioBuscado.Foto),
                    new Claim(ClaimTypes.Role, (usuarioBuscado.CargoUsuario!.NivelCargo == 0 ? "Administrador" : (usuarioBuscado.CargoUsuario.NivelCargo == 1 ? "Gerente De Projetos" : "Colaborador"))),
                    new Claim("role", (usuarioBuscado.CargoUsuario!.NivelCargo == 0 ? "Administrador" : (usuarioBuscado.CargoUsuario.NivelCargo == 1 ? "Gerente De Projetos" : "Colaborador"))),
                    new Claim("idColaborador", (colaboradorBuscado != null) ? colaboradorBuscado.Id.ToString() : "")
                };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("projeto-zenith-chave-autenticacao-webapi"));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "webapi.projeto.zenith",
                    audience: "webapi.projeto.zenith",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(50),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
