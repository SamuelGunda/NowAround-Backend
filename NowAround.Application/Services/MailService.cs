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
    
    public async Task SendWelcomeEmailAsync(string name, string establishmentName,string receiver)
    {
        try
        {
            using var client = new SmtpClient(SmtpServer, SmtpPort);
            
            var htmlBody = """
                           
                                       <html lang="en">
                                           <head>
                                               <meta charset="UTF-8">
                                               <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                               <title>Your Email Template</title>
                           """ + """
                                                     <style>
                                                         @media only screen and (max-width: 600px) {
                                                             .container {
                                                                 width: 100% !important;
                                                             }
                                                             .content {
                                                                 padding: 20px !important;
                                                             }
                                                         }
                                                     </style>
                            """ + $"""
                                            </head>
                                            <body style="margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: hsl(0, 0%, 98%);">
                                                <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                                    <tr>
                                                        <td align="center" style="padding: 40px 0;">
                                                            <table role="presentation" class="container" style="width: 600px; border-collapse: collapse; background-color: hsl(0, 0%, 98%); box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);">
                                                                <tr>
                                                                    <td style="padding: 20px; background-color: hsl(17.8, 100%, 71%);">
                                                                        <h1 style="color: hsl(0, 1.1%, 17.8%); margin: 0; font-size: 28px;">nowAround</h1>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding-top:20px;  text-align: center;">
                                                                      <img src="https://nowaround.site/logo/logo.png" style="width: 8rem; height: 8rem alt="">
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="content" style="padding: 40px;">
                                                                        <h2 style="color: hsl(0, 1.1%, 17.8%); margin-top: 0;">We are happy to have you!</h2>
                                                                        <p style="color: hsl(0, 1.1%, 17.8%); line-height: 1.5;">
                                                                            Hello {name},<br><br>
                                                                            Thank you for registering your {establishmentName} at our website, we are currently working on evaluating your request, and we will be in touch.
                                                                        </p>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding: 20px; background-color: hsl(17.8, 100%, 71%); text-align: center;">
                                                                        <p style="color: hsl(0, 1.1%, 17.8%); margin: 0; font-size: 14px;">
                                                                            Â© 2025 nowAround. All rights reserved.<br>
                                                                            You're receiving this email because you registered an establishment.
                                                                        </p>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
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
    
    public async Task SendAccountAcceptedEmailAsync(string name, string establishmentName, string receiver, string password)
    {
        try
        {
            using var client = new SmtpClient(SmtpServer, SmtpPort);

            var htmlBody = """
                           
                             <html lang="en">
                             <head>
                                 <meta charset="UTF-8">
                                 <meta name="viewport" content="width=device-width, initial-scale=1.0">
                                 <title>Your Establishment Registration Accepted</title>
                           """ + """
                                 <style>
                                     @media only screen and (max-width: 600px) {
                                       "  .container {
                                             width: 100% !important;
                                         }
                                         .content {
                                             padding: 20px !important;
                                         }
                                     }
                                 </style>
                            """ + $"""
                             </head>
                             <body style="margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: hsl(0, 0%, 98%);">
                                 <table role="presentation" style="width: 100%; border-collapse: collapse;">
                                     <tr>
                                         <td align="center" style="padding: 40px 0;">
                                             <table role="presentation" class="container" style="width: 600px; border-collapse: collapse; background-color: hsl(0, 0%, 98%); box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);">
                                                 <tr>
                                                     <td style="padding: 20px; background-color: hsl(17.8, 100%, 71%);">
                                                         <h1 style="color: hsl(0, 1.1%, 17.8%); margin: 0; font-size: 28px;">nowAround</h1>
                                                     </td>
                                                 </tr>
                                                 <tr>
                                                     <td style="padding-top:20px;  text-align: center;">
                                                       <img src="https://nowaround.site/logo/logo.png" style="width: 8rem; height: 8rem;" alt="nowAround Logo">
                                                     </td>
                                                 </tr>
                                                 <tr>
                                                     <td class="content" style="padding: 40px;">
                                                         <h2 style="color: hsl(0, 1.1%, 17.8%); margin-top: 0;">Great news! Your establishment has been accepted!</h2>
                                                         <p style="color: hsl(0, 1.1%, 17.8%); line-height: 1.5;">
                                                             Hello {name},<br><br>
                                                             We're excited to inform you that your establishment, {establishmentName}, has been successfully registered and accepted on nowAround. You can now start using our platform to showcase your business.
                                                         </p>
                                                         <p style="color: hsl(0, 1.1%, 17.8%); line-height: 1.5;">
                                                             Here are your login credentials:
                                                         </p>
                                                         <table style="width: 100%; border-collapse: collapse; margin-bottom: 20px;">
                                                             <tr>
                                                                 <td style="padding: 10px; background-color: hsl(17.8, 100%, 71%); color: hsl(0, 1.1%, 17.8%); font-weight: bold;">Email:</td>
                                                                 <td style="padding: 10px; border: 1px solid hsl(17.8, 100%, 71%); color: hsl(0, 1.1%, 17.8%);">{receiver}</td>
                                                             </tr>
                                                             <tr>
                                                                 <td style="padding: 10px; background-color: hsl(17.8, 100%, 71%); color: hsl(0, 1.1%, 17.8%); font-weight: bold;">Password:</td>
                                                                 <td style="padding: 10px; border: 1px solid hsl(17.8, 100%, 71%); color: hsl(0, 1.1%, 17.8%);">{password}</td>
                                                             </tr>
                                                         </table>
                                                         <p style="color: hsl(0, 1.1%, 17.8%); line-height: 1.5;">
                                                             For security reasons, we recommend changing your password after your first login. You can do this in your account settings.
                                                         </p>
                                                         <p style="color: hsl(0, 1.1%, 17.8%); line-height: 1.5;">
                                                             Welcome aboard, and we look forward to seeing your establishment thrive on nowAround!
                                                         </p>
                                                     </td>
                                                 </tr>
                                                 <tr>
                                                     <td style="padding: 20px; background-color: hsl(17.8, 100%, 71%); text-align: center;">
                                                         <p style="color: hsl(0, 1.1%, 17.8%); margin: 0; font-size: 14px;">
                                                             Â© 2025 nowAround. All rights reserved.<br>
                                                             You're receiving this email because your establishment registration has been accepted.
                                                         </p>
                                                     </td>
                                                 </tr>
                                             </table>
                                         </td>
                                     </tr>
                                 </table>
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
            _logger.LogError(e, "Failed to acceptance email to {Email}", receiver);
            throw new Exception("Failed to send acceptance email", e);
        }
    }
}