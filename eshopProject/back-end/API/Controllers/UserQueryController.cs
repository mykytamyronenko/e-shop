using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Application.Queries.getByName;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class UserQueryController:ControllerBase
{
    private readonly UsersQueryProcessor _usersQueryProcessor;

    public UserQueryController(UsersQueryProcessor usersQueryProcessor)
    {
        _usersQueryProcessor = usersQueryProcessor;
    }
    
    [HttpGet("users")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public List<UsersGetAllOutput.Users> GetAll()
    {
        return _usersQueryProcessor.GetAll(null!).UsersList;
    }
    
    
    [HttpGet("users/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UsersGetByIdOutput> GetById(int id)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        var userTmp = _usersQueryProcessor.GetById(id);
        if (userTmp.UserId != userIdFromToken && !User.IsInRole("admin"))
        {
            return Forbid();
        }
        if (id < 0)
        {
            return BadRequest("User ID must be greater than zero."); // Retourne 400
        }

        try
        {
            var userOutput = _usersQueryProcessor.GetById(id);
            return Ok(userOutput); // Retourne 200 avec les données de l'utilisateur
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message); // Retourne 404 avec le message de l'exception
        }

    }
    
    [HttpGet("users/findUser")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UsersGetByUsernameOutput> GetByUsername(string name)
    {
        try
        {
            var userOutput = _usersQueryProcessor.GetByUsername(name);
            return Ok(userOutput);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message);
        }

    }
    
    [HttpGet("users/getUsername")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult GetUsernameById(int id)
    {
        try
        {
            var userOutput = _usersQueryProcessor.GetById(id);
            var username = userOutput.Username;
            return Ok(new { username = username });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    
}