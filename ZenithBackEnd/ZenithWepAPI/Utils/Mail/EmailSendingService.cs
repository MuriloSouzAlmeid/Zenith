using Microsoft.AspNetCore.Mvc;

namespace ZenithWepAPI.Utils.Mail
{
    public class EmailSendingService
    {
        private readonly IEmailService emailService;
        public EmailSendingService(IEmailService service)
        {
            emailService = service;
        }

        /// <summary>
        /// Método para envio de email de boas vindas ao cadastrar um novo usuário
        /// </summary>
        /// <param name="email">Email para qual será enviado</param>
        /// <param name="userName">Nome do usuário</param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendWelcomeEmail(string email, string userName)
        {
            try
            {
                //monta a requisição passando os dados do email
                MailRequest request = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Bem vindo ao Zenith",
                    Body = GetHtmlContentWelcome(userName)
                };

                //chama o método para enviar a requisição e consequentemente o email
                await emailService.SendEmailAsync(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Método para enviar um email de recuperação de senha
        /// </summary>
        /// <param name="email"></param>
        /// <param name="codigo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendRecovery(string email, string codigo)
        {
            try
            {
                //monta a requisição passando os dados do email
                MailRequest request = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Recuperar a sua senha Zenith",
                    Body = GetHtmlContentRecovery(codigo)
                };

                //chama o método para enviar a requisição e consequentemente o email
                await emailService.SendEmailAsync(request);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Método para email de boas vindas
        private string GetHtmlContentWelcome(string userName)
        {
            //caminho imagem original
            //string imagemVoyager = "https://voyagerblobstorage.blob.core.windows.net/voyagercontainerblob/LogoVoyager.png";

            //caminho imagem poggers

            // Constrói o conteúdo HTML do e-mail, incluindo o nome do usuário
            string Conteudo = @"
<div style=""width:100%; background-color:#8531C6; padding: 20px;"">
    <div style=""max-width: 600px; margin: 0 auto; background-color:#FFFFFF; border-radius: 10px; padding: 20px;""> 
        <h1 style=""color: #333333; text-align: center; margin-top: 10px;"">Bem-vindo ao Zenith! &#127758;✈️</h1>
        <p style=""color: #666666; text-align: center;"">Olá <strong>" + userName + @"</strong>,</p>
        <p style=""color: #666666;text-align: center"">Estamos muito felizes por ter você conosco.</p>
        <p style=""color: #666666;text-align: center""><br>Atenciosamente, Equipe Zenith</p>
    </div>
</div>";

            // Retorna o conteúdo HTML do e-mail
            return Conteudo;
        }


        //Método para email de recuperação de senha
        private string GetHtmlContentRecovery(string codigo)
        {
            string Response = @"
<div style=""width:100%; background-color:#8531C6; padding: 20px;"">
    <div style=""max-width: 600px; margin: 0 auto; background-color:#FFFFFF; border-radius: 10px; padding: 20px;"">
        <h1 style=""color: #333333;text-align: center; margin-top: 10px;"">Recuperação de senha</h1>
        <p style=""color: #666666;font-size: 24px; text-align: center;"">Código de confirmação <strong>" + codigo + @"</strong></p>
    </div>
</div>";

            return Response;
        }
    }
}
