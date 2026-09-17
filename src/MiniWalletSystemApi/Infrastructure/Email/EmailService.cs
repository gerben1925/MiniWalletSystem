using System.Net;
using System.Net.Mail;
using MiniWalletSystemApi.Interfaces.infrastructure.Configuration;
using MiniWalletSystemApi.Interfaces.infrastructure.Email;

namespace MiniWalletSystemApi.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IAppSettingProvider _appSettingProvider;
   
    public EmailService(IAppSettingProvider appSettingProvider)
    {
        _appSettingProvider = appSettingProvider;
    }  
        
    public async Task SendEmailAsync(string to, string subject, string body)
    {
           
                
        using var smtp = new SmtpClient
        {
            Host = _appSettingProvider.GetString("SmtpCredentials:Host") ?? string.Empty,
            Port = _appSettingProvider.GetInt("SmtpCredentials:Port"),
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _appSettingProvider.GetString("SmtpCredentials:NetworkCredentialUsername") ?? string.Empty,
                _appSettingProvider.GetString("SmtpCredentials:NetworkCredentialPass") ?? string.Empty
            )

        };

        var message = new MailMessage
        {
            From = new MailAddress(_appSettingProvider.GetString("SmtpCredentials:FROMEmail") ?? string.Empty),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);

        await smtp.SendMailAsync(message);
    }
}