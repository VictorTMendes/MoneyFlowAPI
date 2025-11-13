using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace MoneyFlowAPI.services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarEmailAsync(string para, string assunto, string mensagemHtml)
        {
            var smtpServer = _config["Email:SmtpServer"];
            var smtpPort = int.Parse(_config["Email:SmtpPort"]);
            var smtpUser = _config["Email:SmtpUser"];
            var smtpPass = _config["Email:SmtpPass"];
            var smtpFrom = _config["Email:From"];

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                var mail = new MailMessage
                {
                    From = new MailAddress(smtpFrom, "MoneyFlow Suporte"),
                    Subject = assunto,
                    Body = mensagemHtml,
                    IsBodyHtml = true
                };
                mail.To.Add(para);

                await client.SendMailAsync(mail);
            }
        }
    }
}
