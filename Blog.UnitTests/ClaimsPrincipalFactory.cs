using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Blog.Core.Entities;

public static class ClaimsPrincipalFactory
{
    public static ClaimsPrincipal CreateAnonymous()
    {
        var identity = new ClaimsIdentity();
        return new ClaimsPrincipal(identity);
    }

    public static ClaimsPrincipal CreateUser(ApplicationUser user)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            };
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        return new ClaimsPrincipal(identity);
    }
}