using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NowAround.Application.Services;

public class MailService : IMailService
{
    private readonly ILogger<MailService> _logger;
    
    private const string SmtpServer = "smtp.gmail.com";
    private const int SmtpPort = 587;
    private const string Sender = "now.around.site@gmail.com";
    
    private readonly string _senderPassword;
    
    public MailService(IConfiguration configuration, ILogger<MailService> logger)
    {
        _logger = logger;
        
        _senderPassword = configuration["Email:Password"] ?? throw new ArgumentException("Email password is missing");
    }
    
    public async Task SendWelcomeEmailAsync(string name, string receiver)
    {
        try
        {
            using var client = new SmtpClient(SmtpServer, SmtpPort);
            
            var htmlBody = $"""

            <html>
                <body>
                    <h1 style='color:blue;'>Welcome {name} to .NET Emailing!</h1>
                    <p>This is a prototype email with basic <b>HTML formatting</b>.</p>
                    <p>Regards,<br/>Your Friendly .NET App</p>
                </body>
            </html>

            """;
                
            client.Credentials = new NetworkCredential(Sender, _senderPassword);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress("now.around.site@gmail.com"),
                Subject = "Welcome!",
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(receiver);

            await client.SendMailAsync(mailMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send welcome email to {Email}", receiver);
            throw new Exception("Failed to send welcome email", e);
        }
    }
}