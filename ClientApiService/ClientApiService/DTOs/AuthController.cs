using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClientApiService.DTOs;
using ClientApiService.Models;
using ClientApiService.Services;

namespace ClientApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthService _authService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        AuthService authService)
    {
        _userManager = userManager;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, model.Role);

        return Ok(new { Message = "User created successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized("Invalid credentials");

        if (!user.IsActive)
            return Unauthorized("Account is disabled");

        var token = await _authService.GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token!,
            Email = user.Email!,
            Role = roles.FirstOrDefault()!,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }
}