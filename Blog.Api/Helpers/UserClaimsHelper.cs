using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Blog.Api.Helpers;
public static class UserClaimsHelper
{
    public static (Guid UserId, string Author) GetUserDetails(ClaimsPrincipal User)
    {
        var userId = (User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value) ?? throw new UnauthorizedAccessException("User is not authorized");
        var author = (User.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Name)?.Value) ?? throw new UnauthorizedAccessException("User is not authorized");
        return (Guid.Parse(userId), author);
    }
}