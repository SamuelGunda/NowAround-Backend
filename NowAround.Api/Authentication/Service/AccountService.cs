using NowAround.Api.Authentication.Interfaces;

namespace NowAround.Api.Authentication.Service;

public class AccountService() : IAccountService
{
    public Task<string> CheckIfAccountExistAsync(int auth0Id)
    {
        throw new NotImplementedException();
    }
}