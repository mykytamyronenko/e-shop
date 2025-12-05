using Application.Dtos;
using Application.Services;
using AutJwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    
    private readonly UserService _userService;


    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<DtoOutputConnectedUser> Login(DtoInputUser user)
    {
        var token = _userService.Login(user);

        if (token == null)
        {
            return Unauthorized("Email, username or password incorrect.");
        }

        Response.Cookies.Append("cookie", token, new CookieOptions
        {
            Secure = true,
            HttpOnly = false
        });

        return Ok(new DtoOutputConnectedUser
        {
            Token = token
        });
    }
    
    [HttpPost("logout")]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("cookie");
        return Ok(new { message = "Logged out successfully" });
    }
}