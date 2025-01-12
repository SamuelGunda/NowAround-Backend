namespace NowAround.Api.Services.Interfaces;

public interface IMailService
{
    Task SendWelcomeEmailAsync(string name, string receiver);
}