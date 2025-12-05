using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class UserCommandsController: ControllerBase
{
    private readonly UserCommandsProcessor _userCommandsProcessor;
    private readonly UsersQueryProcessor _usersQueryProcessor;

    public UserCommandsController(UserCommandsProcessor userCommandsProcessor, UsersQueryProcessor usersQueryProcessor)
    {
        _userCommandsProcessor = userCommandsProcessor;
        _usersQueryProcessor = usersQueryProcessor;
    }
    

    [HttpPost("users")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<UserCreateOutput> Create([FromForm] BasicUserCreateCommand command)
    {
        if (command == null)
        {
            return BadRequest("Invalid user data."); // Return 400
        }
        try
        {
            var result = _userCommandsProcessor.CreateUser(command);
            return Ok(result); // Return 200
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    
    [HttpPost("usersAdmin")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<UserCreateOutput> CreateAdmin([FromForm] UserCreateCommand command)
    {
        if (command == null)
        {
            return BadRequest("Invalid user data."); // Return 400
        }
        try
        {
            var result = _userCommandsProcessor.CreateAdmin(command);
            return Ok(result); // Return 200
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    
    [HttpDelete("users/{userId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteUser(int userId)
    {
        try
        {
            _userCommandsProcessor.DeleteUser(userId);
            return NoContent(); // Return 204
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID {userId} not found."); // Return 404
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    [HttpPut("users")]
    [Authorize]
    public IActionResult UpdateUser2(UserUpdateCommand command)
    {
        _userCommandsProcessor.UpdateUser(command);
        return Ok("User updated.");
    }
    
    [HttpPut("users/{userId}/status")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateUserStatus(int userId, [FromBody] UserUpdateCommandStatus input)
    {
        var user = _usersQueryProcessor.GetById(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var updateArticleCommand = new UserUpdateCommand()
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            ProfilePicture = user.ProfilePicture,
            MembershipLevel = user.MembershipLevel,
            Rating = user.Rating,
            Status = "deleted",
            Balance = user.Balance,

        };

        _userCommandsProcessor.UpdateUser(updateArticleCommand);

        return Ok(new { message = "User removed successfully.", newStatus = input.Status });
    }
    
}