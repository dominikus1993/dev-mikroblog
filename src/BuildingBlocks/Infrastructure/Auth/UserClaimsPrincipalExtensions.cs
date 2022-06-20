using System.Security.Claims;

namespace DevMikroblog.BuildingBlocks.Infrastructure.Auth;

public static class UserClaimsPrincipalExtensions
{
    public static Guid UserId(this ClaimsPrincipal user)
    {
        if (Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException();
    }
    
    public static string UserName(this ClaimsPrincipal user)
    {
        var username = user.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username))
        {
            return "noname";
        }

        return username;
    }
}