using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class TransactionQueryController: ControllerBase
{
    private readonly TransactionsQueryProcessor _transactionsQueryProcessor;

    public TransactionQueryController(TransactionsQueryProcessor transactionsQueryProcessor)
    {
        _transactionsQueryProcessor = transactionsQueryProcessor;
    }
    
    [HttpGet("transactions")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public List<TransactionsGetAllOutput.Transactions> GetAllTransactions()
    {
        return _transactionsQueryProcessor.GetAll(null!).TransactionsList;
    }
    
    [HttpGet("transactions/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TransactionsGetByIdOutput> GetByIdTransaction(int id)
    {
        if (id < 0)
        {
            return BadRequest("The transaction ID must be more than zero."); // Retourne 400
        }

        try
        {
            var transactionOutput = _transactionsQueryProcessor.GetById(id);
            return Ok(transactionOutput); // Retourne 200 avec les données de l'utilisateur
        }
        catch (TransactionNotFoundException ex)
        {
            return NotFound(ex.Message); // Retourne 404 avec le message de l'exception
        }

    }
}