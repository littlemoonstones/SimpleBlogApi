using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Blog.Core.Entities;
using Blog.Core.Dtos;
using Blog.Core.IService;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    public AuthController(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return Ok($"User: {model.UserName} created successfully");
        }
        else{
            // password might not meet the requirements, so we need to send the errors
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new {
                Errors = errors
            });
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("User's email or password is incorrect");
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, false, false);
        if (result.Succeeded)
        {
            var token = _jwtService.GenerateJwtToken(user!);
            return Ok(token);
        }
        return BadRequest("User's email or password is incorrect");
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

}