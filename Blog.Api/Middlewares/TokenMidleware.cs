using System.IdentityModel.Tokens.Jwt;
using Blog.Core.IService;

public class TokenLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtService _jwtService;

    public TokenLoggingMiddleware(RequestDelegate next, IJwtService jwtService)
    {
        _next = next;
        _jwtService = jwtService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            Console.WriteLine($"Header: {authHeader}");

            var token = authHeader.ToString().Replace("Bearer ", ""); // Remove the 'Bearer ' prefix
            Console.WriteLine($"Token: {token}");


            bool isValid = _jwtService.ValidateToken(token);

            Console.WriteLine($"Token is valid: {isValid}");

            if (isValid)
            {
                DecodeToken(token);
            }
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }

    public void DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token);
        var decodedJwt = jsonToken as JwtSecurityToken;

        Console.WriteLine($"Token ID: {decodedJwt.Id}");
        Console.WriteLine($"Token Audience: {decodedJwt.Audiences.FirstOrDefault()}");
        Console.WriteLine($"Token Issuer: {decodedJwt.Issuer}");
        Console.WriteLine($"Token Valid From: {decodedJwt.ValidFrom}");
        Console.WriteLine($"Token Valid To: {decodedJwt.ValidTo}");

        foreach (var claim in decodedJwt.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }
    }
}