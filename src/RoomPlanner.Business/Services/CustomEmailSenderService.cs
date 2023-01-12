using RoomPlanner.Core.Interfaces;
using RoomPlanner.Infrastructure.Adapters;
using RoomPlanner.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlanner.Business.Services
{
    public class CustomEmailSenderService
    {
        public const string PasswordResetPath = "Templates/PasswordReset.html";
        public const string ActivationPath = "Templates/Activation.html";
        public const string MailConfirmationPath = "Templates/MailConfirmation.html";
        public const string UserOnboardingPath = "Templates/UserOnboarding.html";

        private readonly IEmailAdapter emailAdapter;
        private readonly SmtpServerOptions smtpServerOptions;

        public CustomEmailSenderService(IEmailAdapter emailAdapter, SmtpServerOptions smtpServerOptions)
        {
            this.emailAdapter = emailAdapter;
            this.smtpServerOptions = smtpServerOptions;
        }

        public async Task SendPasswordResetMailAsync(string to, string url)
        {
            string mailbody = File.ReadAllText(PasswordResetPath);

            using (MailMessage message = new MailMessage
            {
                Subject = "Password reset",
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress(smtpServerOptions.SenderMail)
            })
            {
                mailbody = mailbody.Replace("{resetlink}", url);
                mailbody = mailbody.Replace("{year}", DateTime.UtcNow.Year.ToString());

                using (AlternateView alternateViewHtml = AlternateView.CreateAlternateViewFromString(mailbody, Encoding.UTF8, MediaTypeNames.Text.Html))
                {
                    message.AlternateViews.Add(alternateViewHtml);

                    message.To.Add(to.Replace(';', ',')); //replace ; as it just accepts ,

                    await emailAdapter.SendEmailAsync(message);
                }
            }
        }

        public async Task SendRegistrationMailAsync(string to, string url)
        {
            string mailbody = File.ReadAllText(ActivationPath);

            using (MailMessage message = new MailMessage
            {
                Subject = "Account activation",
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress(smtpServerOptions.SenderMail)
            })
            {
                mailbody = mailbody.Replace("{activationlink}", url);
                mailbody = mailbody.Replace("{year}", DateTime.UtcNow.Year.ToString());

                using (AlternateView alternateViewHtml = AlternateView.CreateAlternateViewFromString(mailbody, Encoding.UTF8, MediaTypeNames.Text.Html))
                {
                    message.AlternateViews.Add(alternateViewHtml);

                    message.To.Add(to.Replace(';', ',')); //replace ; as it just accepts ,

                    await emailAdapter.SendEmailAsync(message);
                }
            }
        }

        public async Task SendInvitationMail(string to, string url)
        {
            string mailbody = File.ReadAllText(UserOnboardingPath);

            using (MailMessage message = new MailMessage
            {
                Subject = "Account activation",
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress(smtpServerOptions.SenderMail)
            })
            {
                mailbody = mailbody.Replace("{activationlink}", url);
                mailbody = mailbody.Replace("{year}", DateTime.UtcNow.Year.ToString());

                using (AlternateView alternateViewHtml = AlternateView.CreateAlternateViewFromString(mailbody, Encoding.UTF8, MediaTypeNames.Text.Html))
                {
                    message.AlternateViews.Add(alternateViewHtml);

                    message.To.Add(to.Replace(';', ',')); //replace ; as it just accepts ,

                    await emailAdapter.SendEmailAsync(message);
                }
            }
        }

        public async Task SendMailConfirmationAsync(string to, string url)
        {
            string mailbody = File.ReadAllText(MailConfirmationPath);

            using (MailMessage message = new MailMessage
            {
                Subject = "Mail confirmation",
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress(smtpServerOptions.SenderMail)
            })
            {
                mailbody = mailbody.Replace("{activationlink}", url);
                mailbody = mailbody.Replace("{year}", DateTime.UtcNow.Year.ToString());

                using (AlternateView alternateViewHtml = AlternateView.CreateAlternateViewFromString(mailbody, Encoding.UTF8, MediaTypeNames.Text.Html))
                {
                    message.AlternateViews.Add(alternateViewHtml);

                    message.To.Add(to.Replace(';', ',')); //replace ; as it just accepts ,

                    await emailAdapter.SendEmailAsync(message);
                }
            }
        }
    }
}
