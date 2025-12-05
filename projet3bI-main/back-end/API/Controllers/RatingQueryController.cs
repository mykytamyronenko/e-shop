using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class RatingQueryController: ControllerBase
{
    public readonly RatingsQueryProcessor _ratingsQueryProcessor;

    public RatingQueryController(RatingsQueryProcessor ratingQueryProcessor)
    {
        _ratingsQueryProcessor = ratingQueryProcessor;
    }
    
    [HttpGet("ratings")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public List<RatingsGetAllOutput.Ratings> GetAllRatings()
    {
        return _ratingsQueryProcessor.GetAll(null!).RatingsList;
    }
    
    [HttpGet("ratings/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<RatingsGetByIdOutput> GetByIdRating(int id)
    {
        if (id < 0)
        {
            return BadRequest("The rating ID must be more than zero."); // Retourne 400
        }

        try
        {
            var ratingOutput = _ratingsQueryProcessor.GetById(id);
            return Ok(ratingOutput); // Retourne 200 avec les données de l'utilisateur
        }
        catch (RatingNotFoundException ex)
        {
            return NotFound(ex.Message); // Retourne 404 avec le message de l'exception
        }

    }
}