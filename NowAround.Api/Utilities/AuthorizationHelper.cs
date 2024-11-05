using System.Security.Claims;

namespace NowAround.Api.Utilities;

public static class AuthorizationHelper
{
    public static bool HasAdminOrMatchingEstablishmentId(ClaimsPrincipal user, string requestedAuth0Id)
    {
        var roles = user.Claims.Where(c => c.Type == "roles").Select(c => c.Value);
        var auth0Id = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        
        return roles.Contains("Admin") || (roles.Contains("Establishment") && auth0Id == requestedAuth0Id);
    }
}