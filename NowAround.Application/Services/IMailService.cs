namespace NowAround.Application.Services;

public interface IMailService
{
    Task SendWelcomeEmailAsync(string name, string establishmentName, string receiver);
    Task SendAccountAcceptedEmailAsync(string name, string establishmentName, string receiver, string password);
}