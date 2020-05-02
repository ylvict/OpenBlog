using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Niusys;

namespace OpenBlog.Web.Services.EmailServices
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSetting _emailConfiguration;

        public EmailService(IOptions<EmailSetting> emailConfigurationOptions,ILogger<EmailService> logger)
        {
            if (emailConfigurationOptions is null)
            {
                throw new System.ArgumentNullException(nameof(emailConfigurationOptions));
            }

            _logger = logger;

            _emailConfiguration = emailConfigurationOptions.Value;
        }

        public async Task SendEmailAsync(string emailReceivers, string subject, string content)
        {
            if (_emailConfiguration.SenderName.IsNullOrWhitespace()
                || _emailConfiguration.SenderEmail.IsNullOrWhitespace()
                || _emailConfiguration.SmtpServer.IsNullOrWhitespace())
            {
                _logger.LogWarning($"Email SMTP Setting missing");
                return;
            }
            
            var emailMessage = new EmailMessage()
            {
                Subject = subject,
                Content = content,
                FromAddresses = new List<MailboxAddress>() { new MailboxAddress(_emailConfiguration.SenderName, _emailConfiguration.SenderEmail) },
                ToAddresses = new List<MailboxAddress>()
            };

            foreach (var item in emailReceivers.Split(',', ';'))
            {
                emailMessage.ToAddresses.Add(new MailboxAddress(item, item));
            }

            if (emailMessage.ToAddresses.Count > 0)
            {
                await SendAsync(emailMessage);
            }
        }

        private async Task SendAsync(EmailMessage emailMessage)
        {
            if (emailMessage is null)
            {
                throw new System.ArgumentNullException(nameof(emailMessage));
            }

            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            var sender = emailMessage.FromAddresses.First();
            message.Sender = new MailboxAddress(sender.Name, sender.Address);
            message.Subject = emailMessage.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using (var emailClient = new SmtpClient())
            {
                emailClient.Timeout = 5000; //超时时间设置为5ms

                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
#pragma warning disable CA5359 // 请勿禁用证书验证
                emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
#pragma warning restore CA5359 // 请勿禁用证书验证

                //The last parameter here is to use SSL (Which you should!)
                await emailClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, SecureSocketOptions.Auto);

                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                await emailClient.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                await emailClient.SendAsync(message);

                await emailClient.DisconnectAsync(true);
            }
        }

    }
}