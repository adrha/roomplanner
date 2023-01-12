using RoomPlanner.Core.Interfaces;
using RoomPlanner.Options;
using System.Net;
using System.Net.Mail;

namespace RoomPlanner.Infrastructure.Adapters
{
    public class SmtpEmailAdapter : IEmailAdapter
    {
        private readonly SmtpServerOptions smtpServerOptions;

        public SmtpEmailAdapter(SmtpServerOptions smtpServerOptions)
        {
            this.smtpServerOptions = smtpServerOptions;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using MailMessage mail = new MailMessage
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlMessage,
                From = new MailAddress(smtpServerOptions.SenderMail)
            };
            mail.To.Add(email);

            using SmtpClient smtpClient = new SmtpClient
            {
                Port = smtpServerOptions.Port,
                EnableSsl = smtpServerOptions.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = smtpServerOptions.Host
            };

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpServerOptions.UserName, smtpServerOptions.Password);

            await smtpClient.SendMailAsync(mail);
        }

        public async Task SendEmailAsync(MailMessage message)
        {
            using SmtpClient smtpClient = new SmtpClient
            {
                Port = smtpServerOptions.Port,
                EnableSsl = smtpServerOptions.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = smtpServerOptions.Host
            };

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpServerOptions.UserName, smtpServerOptions.Password);

            await smtpClient.SendMailAsync(message);
        }
    }
}
