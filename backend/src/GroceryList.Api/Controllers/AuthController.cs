using GroceryList.Application.DTOs;
using GroceryList.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroceryList.Api.Controllers;

/// <summary>
/// Public endpoints: register (user story 1) and login (user story 2).
/// Logout (user story 8) needs no endpoint: the client simply deletes its token.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var user = await authService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        return Ok(await authService.LoginAsync(request));
    }
}
