using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class UserMembershipCommandsController:ControllerBase
{
    private readonly UserMembershipCommandsProcessor _userMembershipCommandsProcessor;
    private readonly UserMembershipsQueryProcessor _userMembershipsQueryProcessor;

    public UserMembershipCommandsController(UserMembershipCommandsProcessor userMembershipCommandsProcessor, UserMembershipsQueryProcessor userMembershipsQueryProcessor)
    {
        _userMembershipCommandsProcessor = userMembershipCommandsProcessor;
        _userMembershipsQueryProcessor = userMembershipsQueryProcessor;
    }
    
    [HttpPost("userMemberships")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<UserMembershipCreateOutput> CreateUserMembership(UserMembershipCreateCommand command)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        if (command.UserId != userIdFromToken)
        {
            return Forbid();
        }
        
        if (command == null)
        {
            return BadRequest("Invalid user membership data."); // Return 400
        }
        try
        {
            var result = _userMembershipCommandsProcessor.CreateUserMembership(command);
            return Ok(result); // Return 200
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    
    [HttpPut("userMemberships/{usermembershipId}")]
    [Authorize]
    public IActionResult UpdateUserMembership(UserMembershipUpdateCommand command)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        if (command.UserId != userIdFromToken)
        {
            return Forbid();
        }
        _userMembershipCommandsProcessor.UpdateUserMembership(command);
        return Ok("User Membership updated.");
    }
    
    [HttpDelete("userMemberships/{userMembershipId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteUserMembership(int userMembershipId)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        var usermembershipTmp = _userMembershipsQueryProcessor.GetById(userMembershipId);
        if (usermembershipTmp.UserId != userIdFromToken)
        {
            return Forbid();
        }
        
        try
        {
            _userMembershipCommandsProcessor.DeleteUserMembership(userMembershipId);
            return NoContent(); // Return 204
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User membership with ID {userMembershipId} not found."); // Return 404
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
}