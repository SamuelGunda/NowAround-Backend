namespace NowAround.Application.Services;

public interface IMailService
{
    Task SendWelcomeEmailAsync(string name, string receiver);
}