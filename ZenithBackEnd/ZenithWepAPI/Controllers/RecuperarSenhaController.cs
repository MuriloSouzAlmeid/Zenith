using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenithWepAPI.Context;
using ZenithWepAPI.Domains;
using ZenithWepAPI.Utils;
using ZenithWepAPI.Utils.Mail;
using ZenithWepAPI.ViewModels;

namespace ZenithWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RecuperarSenhaController : ControllerBase
    {
        private readonly EmailSendingService _emailService;
        private readonly ZenithContext _context;

        public RecuperarSenhaController(EmailSendingService service)
        {
            _emailService = service;
            _context = new ZenithContext();
        }

        [HttpPost("EnviarEmail")]
        public async Task<IActionResult> SendMail(EmailViewModel Email)
        {
            try
            {
                Usuario usuarioBuscado = _context.Usuario.FirstOrDefault(u => u.Email == Email.Email)!;

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuario não encontrado");
                }

                //Gerar um código com 4 algarismos

                Random random = new Random();
                int recoveryCode = random.Next(1000, 9999);

                usuarioBuscado.CodRecupSenha = recoveryCode.ToString();

                await _context.SaveChangesAsync();

                await _emailService.SendRecovery(usuarioBuscado.Email!, recoveryCode.ToString());

                return Ok("Email enviado com sucesso");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpPost("ValidarCodigoRecuperacao")]
        public async Task<IActionResult> ValidateRecoveryCode(ValidarEmailViewModel infoEmailCodigo)
        {
            try
            {
                Usuario usuarioBuscado = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == infoEmailCodigo.Email)!;

                if (usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                if (usuarioBuscado.CodRecupSenha != infoEmailCodigo.Codigo)
                {
                    return BadRequest("Código informado inválido, tente novamente");
                }

                usuarioBuscado.CodRecupSenha = null;

                await _context.SaveChangesAsync();

                return Ok("Código de recupareção válido");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }

        [HttpPut("AtualizarSenhaUsuario")]
        public IActionResult AtualizarSenhaUsuario(LoginViewModel infoNovaSenha)
        {
            try
            {
                Usuario usuarioBuscado = _context.Usuario.FirstOrDefault(u => u.Email == infoNovaSenha.Email)!;

                if(usuarioBuscado == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                usuarioBuscado.Senha = Criptografia.GerarHash(infoNovaSenha.Senha!);

                _context.Usuario.Update(usuarioBuscado);

                _context.SaveChanges();

                return Ok("Senha atualizada com sucesso");
            }
            catch (Exception erro)
            {
                return BadRequest(erro.Message);
            }
        }
    }
}
