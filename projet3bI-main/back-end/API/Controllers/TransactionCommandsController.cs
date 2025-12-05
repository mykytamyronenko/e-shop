using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class TransactionCommandsController: ControllerBase
{
    private readonly TransactionCommandsProcessor _transactionCommandsProcessor;
    private readonly TradesQueryProcessor _tradesQueryProcessor;

    public TransactionCommandsController(TransactionCommandsProcessor transactionCommandsProcessor, TradesQueryProcessor tradesQueryProcessor)
    {
        _transactionCommandsProcessor = transactionCommandsProcessor;
        _tradesQueryProcessor = tradesQueryProcessor;
    }
    
    [HttpPost("transactions")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<TransactionCreateOutput> CreateTransaction(TransactionCreateCommand command)
    {
        
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        if (command.BuyerId != userIdFromToken)
        {
            return Forbid();
        }
        
        if (command == null)
        {
            return BadRequest("Invalid transaction data."); // Return 400
        }

        var alltrade = _tradesQueryProcessor.GetAll(null);
        
        foreach(var magos in alltrade.TradesList)
        {
            if (magos.ReceiverArticleId == command.ArticleId && magos.Status == "in progress")
            {
                return BadRequest("You can not buy an item that is waiting to be traded"); // Return 400
            }

            foreach (var article in magos.TraderArticlesIds)
            {
                if (article == command.ArticleId && magos.Status == "in progress")
                {
                    return BadRequest("You can not buy an item that is waiting to be traded"); // Return 400
                }
            }
        }
        try
        {
            var result = _transactionCommandsProcessor.CreateTransaction(command);
            return Ok(result); // Return 200
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    
    [HttpPut("transactions/{transactionId}")]
    [Authorize]
    public IActionResult UpdateTransaction(TransactionUpdateCommand command)
    {
        _transactionCommandsProcessor.UpdateTransaction(command);
        return Ok("Transaction updated.");
    }
    
    [HttpDelete("transactions/{transactionId}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteTransaction(int transactionId)
    {
        try
        {
            _transactionCommandsProcessor.DeleteTransaction(transactionId);
            return NoContent(); // Return 204
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Transaction with ID {transactionId} not found."); // Return 404
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
}