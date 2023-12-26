using System.Net;
using Microsoft.AspNetCore.Identity;
using Blog.Core.Dtos;
using Blog.Core.Entities;
namespace Blog.Core.IService;
public interface IJwtService
{
    LoginResponseDto GenerateJwtToken(ApplicationUser user);
    bool ValidateToken(string token);
}