using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class UserMembershipQueryController: ControllerBase
{
    private readonly UserMembershipsQueryProcessor _userMembershipsQueryProcessor;

    public UserMembershipQueryController(UserMembershipsQueryProcessor userMembershipsQueryProcessor)
    {
        _userMembershipsQueryProcessor = userMembershipsQueryProcessor;
    }
    
    [HttpGet("userMemberships")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public List<UsersMembershipGetAllOutput.UserMemberships> GetAllUserMemberships()
    {
        return _userMembershipsQueryProcessor.GetAll(null!).UserMembershipsList;
    }


    [HttpGet("userMemberships/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UserMembershipsGetByIdOutput> GetByIdUserMembership(int id)
    {
        if (id < 0)
        {
            return BadRequest("The id of the user membership must be greater than 0."); // Return 400
        }

        try
        {
            var userMembershipOutput = _userMembershipsQueryProcessor.GetById(id);
            return Ok(userMembershipOutput); // return 200
        }
        catch (UserMembershipNotFoundException ex)
        {
            return NotFound(ex.Message); // Return 404
        }

    }
}