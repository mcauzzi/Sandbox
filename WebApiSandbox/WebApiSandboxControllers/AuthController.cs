using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SandboxAuthenticationInterfaces;
using WebApiSandboxViewModels;

namespace WebApiSandboxControllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
    {
        _userManager       = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        var user   = new IdentityUser { UserName = model.Username, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user.UserName,roles);
            return Ok(new { Token = token });
        }

        return Unauthorized();
    }
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }
        return NotFound();
    }
    
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService             _tokenService;
}