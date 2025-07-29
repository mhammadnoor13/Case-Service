using Application.Dtos;
using Application.Interfaces;
using Infrastructure.Options;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Ocsp;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class GmailService : IMailService
    {
        private readonly GmailOptions _gmailOptions;
        private readonly ILogger<GmailService> _logger;

        public GmailService(
            IOptions<GmailOptions> gmailOptions,
            ILogger<GmailService> logger)
        {
            _gmailOptions = gmailOptions.Value;
            _logger = logger;

        }

        public async Task SendSolutionMailAsync(SendMailRequest sendMailRequest, CancellationToken ct)
        {
            _logger.LogInformation("🔔 [SMTP] Preparing to send email to {Recipient}", sendMailRequest.Recipient);

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(MailboxAddress.Parse(_gmailOptions.Email));
            mailMessage.To.Add(MailboxAddress.Parse(sendMailRequest.Recipient));
            mailMessage.Subject = sendMailRequest.Subject;
            mailMessage.Body = new TextPart("plain") { Text = sendMailRequest.Body };

            using var protocolLog = new MailKit.ProtocolLogger(Console.OpenStandardError());


            using var client = new SmtpClient(protocolLog);

            var secureOption = SecureSocketOptions.SslOnConnect;



            _logger.LogInformation("Connecting to {Host}:{Port} (TLS={Option})…",
                _gmailOptions.Host, _gmailOptions.Port, secureOption);



    
            await client.ConnectAsync(_gmailOptions.Host, _gmailOptions.Port, secureOption, ct);

            _logger.LogInformation("Authenticating as {Email}…", _gmailOptions.Email);

            if (_gmailOptions.Host.Equals("smtp.gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Authenticating as {Email}…", _gmailOptions.Email);
                await client.AuthenticateAsync(_gmailOptions.Email, _gmailOptions.Password, ct);
            }
            else
            {
                _logger.LogInformation("Skipping authentication on {Host}", _gmailOptions.Host);
            }


            _logger.LogInformation("Sending message to {Recipient}…", sendMailRequest.Recipient);
            await client.SendAsync(mailMessage, ct);

            _logger.LogInformation("Disconnecting…");
            await client.DisconnectAsync(true, ct);
        }
    }
}
