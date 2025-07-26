using Application.Dtos;
using Application.Interfaces;
using CaseService.API.CaseService.Domain.Entities;
using Infrastructure.Options;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class GmailService : IMailService
    {
        private readonly GmailOptions _gmailOptions;
        public GmailService(IOptions<GmailOptions> gmailOptions)
        {
            _gmailOptions = gmailOptions.Value;
        }

        public async Task SendSolutionMailAsync(SendMailRequest sendMailRequest, CancellationToken ct)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_gmailOptions.Email),
                Subject = sendMailRequest.Subject,
                Body = sendMailRequest.Body,
            };

            mailMessage.To.Add(sendMailRequest.Recipient);

            using var smtpClient = new SmtpClient();
            smtpClient.Host = _gmailOptions.Host;
            smtpClient.Port = _gmailOptions.Port;
            smtpClient.Credentials = new NetworkCredential(
                _gmailOptions.Email, _gmailOptions.Password
                );
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
