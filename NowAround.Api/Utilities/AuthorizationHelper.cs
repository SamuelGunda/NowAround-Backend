using System.Security.Claims;

namespace NowAround.Api.Utilities;

public static class AuthorizationHelper
{
    public static bool HasAdminRightsOrMatchingAuth0Id(ClaimsPrincipal user, string requestedAuth0Id)
    {
        var roles = user.Claims.Where(c => c.Type == "https://now-around-auth-api/roles").Select(c => c.Value);
        var auth0Id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        return roles.Contains("Admin") || auth0Id == requestedAuth0Id;
    }
}