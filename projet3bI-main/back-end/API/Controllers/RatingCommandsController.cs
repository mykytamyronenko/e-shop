using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class RatingCommandsController: ControllerBase
{
    private readonly RatingCommandsProcessor _ratingCommandsProcessor;
    private readonly RatingsQueryProcessor _ratingsQueryProcessor;

    public RatingCommandsController(RatingCommandsProcessor ratingCommandsProcessor, RatingsQueryProcessor ratingsQueryProcessor)
    {
        _ratingCommandsProcessor = ratingCommandsProcessor;
        _ratingsQueryProcessor = ratingsQueryProcessor;
    }
    
    [HttpPost("ratings")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<TransactionCreateOutput> CreateRating(RatingCreateCommand command)
    {
        
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        if (command.ReviewerId != userIdFromToken)
        {
            return Forbid();
        }
        
        if (command == null)
        {
            return BadRequest("Invalid Rating data."); // Return 400
        }
        try
        {
            var result = _ratingCommandsProcessor.CreateRating(command);
            return Ok(result); // Return 200
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    
    [HttpPut("ratings/{ratingId}")]
    [Authorize]
    public IActionResult UpdateRating(RatingUpdateCommand command)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        if (command.ReviewerId != userIdFromToken)
        {
            return Forbid();
        }
        _ratingCommandsProcessor.UpdateRating(command);
        return Ok("Rating updated.");
    }
    
    [HttpDelete("ratings/{ratingId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteRating(int ratingId)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        var ratingTmp = _ratingsQueryProcessor.GetById(ratingId);
        if (ratingTmp.ReviewerId != userIdFromToken)
        {
            return Forbid();
        }
        
        try
        {
            _ratingCommandsProcessor.DeleteRating(ratingId);
            return NoContent(); // Return 204
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Rating with ID {ratingId} not found."); // Return 404
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
}