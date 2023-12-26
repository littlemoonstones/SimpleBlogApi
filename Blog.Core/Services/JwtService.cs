using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Core.Dtos;
using Blog.Core.Entities;
using Blog.Core.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace Blog.Core.Service;
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public LoginResponseDto GenerateJwtToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var _key = _configuration["Jwt:Key"];
        if (_key == null)
            throw new Exception("Jwt:Key is null");
        var key = Encoding.ASCII.GetBytes(_key);
        // print all information of user
        Console.WriteLine($"Jwt User: {user.Id} - {user.UserName} - {user.Email}");

        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // new Claim(ClaimTypes.Email, user.Email),
            // Add additional claims as needed
        };

        var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"]));
        // issuer: _configuration["Jwt:Issuer"],
        // audience: _configuration["Jwt:Audience"],
        // claims: claims,
        // expires: expiration,
        // signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _configuration["Jwt:Audience"],
            Issuer = _configuration["Jwt:Issuer"],
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        // Console.WriteLine($"Create the Token: {token}");
        // Console.WriteLine($"Token is Valid: {ValidateToken(token)}");

        return new LoginResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Token = token,
            Expiration = expiration,
        };
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}"); // 输出异常信息以便调试
            return false;
        }
    }
}