using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace EducNotes.API.Services
{
     public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, IConfiguration config)
        {
            Options = optionsAccessor.Value;
            _config = config;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var apiKey = _config.GetValue<string>("AppSettings:SENDGRID_APIKEY");

            return Execute(apiKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("no-reply@educnotes.com", "EducNotes"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(true, true);

            return client.SendEmailAsync(msg);
        }
    }
}