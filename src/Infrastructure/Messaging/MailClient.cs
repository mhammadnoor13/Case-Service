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
    public class MailClient : IMailService
    {
        private readonly GmailOptions _gmailOptions;
        private readonly ILogger<MailClient> _logger;

        public MailClient(
            IOptions<GmailOptions> gmailOptions,
            ILogger<MailClient> logger)
        {
            _gmailOptions = gmailOptions.Value;
            _logger = logger;

        }

        public async Task SendSolutionMailAsync(SendMailRequest sendMailRequest, CancellationToken ct)
        {

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(MailboxAddress.Parse(_gmailOptions.Email));
            mailMessage.To.Add(MailboxAddress.Parse(sendMailRequest.Recipient));
            mailMessage.Subject = sendMailRequest.Subject;
            mailMessage.Body = new TextPart("plain") { Text = sendMailRequest.Body };

            using var protocolLog = new MailKit.ProtocolLogger(Console.OpenStandardError());


            using var client = new SmtpClient(protocolLog);

            var secureOption = SecureSocketOptions.SslOnConnect;



                _gmailOptions.Host, _gmailOptions.Port, secureOption);



    
            await client.ConnectAsync(_gmailOptions.Host, _gmailOptions.Port, secureOption, ct);



            _logger.LogInformation("Sending message to {Recipient}…", sendMailRequest.Recipient);
            await client.SendAsync(mailMessage, ct);

            _logger.LogInformation("Disconnecting…");
            await client.DisconnectAsync(true, ct);
        }
    }
}
