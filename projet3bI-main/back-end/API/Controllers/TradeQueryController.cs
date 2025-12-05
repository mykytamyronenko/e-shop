using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class TradeQueryController: ControllerBase
{
    private readonly TradesQueryProcessor _tradesQueryProcessor;

    public TradeQueryController(TradesQueryProcessor tradesQueryProcessor)
    {
        _tradesQueryProcessor = tradesQueryProcessor;
    }
    
    [HttpGet("trades")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public List<TradesGetAllOutput.Trades> GetAllTrades()
    {
        return _tradesQueryProcessor.GetAll(null!).TradesList;
    }
    
    [HttpGet("trades/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TradesGetByIdOutput> GetByIdTrade(int id)
    {
        if (id < 0)
        {
            return BadRequest("The trade ID must be more than zero."); // Retourne 400
        }

        try
        {
            var tradeOutput = _tradesQueryProcessor.GetById(id);
            return Ok(tradeOutput); // Retourne 200 avec les données de l'utilisateur
        }
        catch (TradeNotFoundException ex)
        {
            return NotFound(ex.Message); // Retourne 404 avec le message de l'exception
        }

    }
}