using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class MembershipQueryController: ControllerBase
{
    private readonly MembershipsQueryProcessor _membershipsQueryProcessor;

    public MembershipQueryController(MembershipsQueryProcessor membershipsQueryProcessor)
    {
        _membershipsQueryProcessor = membershipsQueryProcessor;
    }
    
    [HttpGet("memberships")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public List<MembershipGetAllOutput.Memberships> GetAllMemberships()
    {
        return _membershipsQueryProcessor.GetAll(null!).MembershipList;
    }


    [HttpGet("memberships/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<MembershipsGetByIdOutput> GetByIdMembership(int id)
    {
        if (id < 0)
        {
            return BadRequest("The id of the membership must be greater than 0."); // Return 400
        }

        try
        {
            var membershipOutput = _membershipsQueryProcessor.GetById(id);
            return Ok(membershipOutput); // return 200
        }
        catch (MembershipNotFoundException ex)
        {
            return NotFound(ex.Message); // Return 404
        }

    }
}