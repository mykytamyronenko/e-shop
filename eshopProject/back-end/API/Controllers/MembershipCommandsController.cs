using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class MembershipCommandsController:ControllerBase
{
    private readonly MembershipCommandsProcessor _membershipCommandsProcessor;

    public MembershipCommandsController(MembershipCommandsProcessor membershipCommandsProcessor)
    {
        _membershipCommandsProcessor = membershipCommandsProcessor;
    }
    
    [HttpPost("memberships")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<MembershipCreateOutput> CreateMembership(MembershipCreateCommand command)
    {
        if (command == null)
        {
            return BadRequest("Invalid membership data."); // Return 400
        }
        try
        {
            var result = _membershipCommandsProcessor.CreateMembership(command);
            return Ok(result); // Return 200
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    
    [HttpPut("memberships/{membershipId}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateMembership(MembershipUpdateCommand command)
    {
        _membershipCommandsProcessor.UpdateMembership(command);
        return Ok("Membership updated.");
    }
    
    [HttpDelete("memberships/{membershipId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteMembership(int membershipId)
    {
        try
        {
            _membershipCommandsProcessor.DeleteMembership(membershipId);
            return NoContent(); // Return 204
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with ID {membershipId} not found."); // Return 404
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
}