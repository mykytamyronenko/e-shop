using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Application.Queries.getById;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class TradeCommandsController: ControllerBase
{
    private readonly TradeCommandsProcessor _tradeCommandsProcessor;
    private readonly TradesQueryProcessor _tradesQueryProcessor;
    private readonly ArticlesQueryProcessor _articlesQueryProcessor;
    private readonly ArticleCommandsProcessor _articleCommandsProcessor;

    public TradeCommandsController(TradeCommandsProcessor tradeCommandsProcessor, TradesQueryProcessor tradesQueryProcessor, ArticlesQueryProcessor articlesQueryProcessor, ArticleCommandsProcessor articleCommandsProcessor)
    {
        _tradeCommandsProcessor = tradeCommandsProcessor;
        _tradesQueryProcessor = tradesQueryProcessor;
        _articlesQueryProcessor = articlesQueryProcessor;
        _articleCommandsProcessor = articleCommandsProcessor;
    }
    
    [HttpPost("trades")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<TransactionCreateOutput> CreateTrade(TradeCreateCommand command)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        if (command.TraderId != userIdFromToken)
        {
            return Forbid();
        }

        var articleIds = command.TraderArticlesIds.Split(',').Select(id => id.Trim()).ToArray();
        
        foreach (var articleIdString in articleIds)
        {
            var article = _articlesQueryProcessor.GetById(int.Parse(articleIdString));
            if (article == null || article.UserId != userIdFromToken)
            {
                return BadRequest($"Article ID {articleIdString} does not belong to the trader.");
            }
        }
        
        if (command == null)
        {
            return BadRequest("Invalid trade data."); // Return 400
        }
        try
        {
            var result = _tradeCommandsProcessor.CreateTrade(command);
            return Ok(result); // Return 200
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    
    [HttpPut("trades/{tradeId}")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateTrade(TradeUpdateCommand command)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        if (command.ReceiverId != userIdFromToken)
        {
            return Forbid();
        }
        
        _tradeCommandsProcessor.UpdateTrade(command);
        return Ok(new { message = "Trade updated." });
    }
    
    [HttpPut("trades/{tradeId}/status")]
    [Authorize]
    public IActionResult UpdateTradeStatus(int tradeId, [FromBody] TradeUpdateCommandStatus input)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        var trade = _tradesQueryProcessor.GetById(tradeId);
        if (trade == null)
        {
            return NotFound("Trade not found.");
        }
        
        if (trade.ReceiverId != userIdFromToken)
        {
            return Forbid("You do not have permission to modify the status of this trade.");
        }
        
        if (input.Status != "accepted" && input.Status != "denied")
        {
            return BadRequest("Invalid status. Allowed values are 'accepted' or 'denied'.");
        }

        if (input.Status == "accepted")
        {
            var traderArticleIdsArray = trade.TraderArticlesIds.Split(',').Select(int.Parse).ToList();
            var traderArticles = new List<ArticlesGetByIdOutput>();
            foreach (var articleId in traderArticleIdsArray)
            {
                var article = _articlesQueryProcessor.GetById(articleId);
                if (article != null)
                {
                    traderArticles.Add(article);
                }
            }
            
            foreach (var article in traderArticles)
            {
                var updateCommand = new ArticleUpdateCommand
                {
                    ArticleId = article.ArticleId,
                    Title = article.Title,
                    Description = article.Description,
                    UserId = trade.ReceiverId, // swap the trader with the receiver
                    Price = article.Price,
                    CreatedAt = article.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,
                    Category = article.Category,
                    Quantity = article.Quantity,
                    State = article.State,
                    Status = article.Status,
                    MainImageUrl = article.MainImageUrl
                };

                _articleCommandsProcessor.UpdateArticle(updateCommand);
            }

            // L'article du receiver est donné au trader
            var receiverArticle = _articlesQueryProcessor.GetById(trade.ReceiverArticleId);
            if (receiverArticle != null)
            {
                var updateCommand = new ArticleUpdateCommand
                {
                    ArticleId = receiverArticle.ArticleId,
                    Title = receiverArticle.Title,
                    Description = receiverArticle.Description,
                    UserId = trade.TraderId, // swap the receiver with the trader
                    Price = receiverArticle.Price,
                    CreatedAt = receiverArticle.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,
                    Category = receiverArticle.Category,
                    Quantity = receiverArticle.Quantity,
                    State = receiverArticle.State,
                    Status = receiverArticle.Status,
                    MainImageUrl = receiverArticle.MainImageUrl
                };
                receiverArticle.UserId = trade.TraderId;
                _articleCommandsProcessor.UpdateArticle(updateCommand);
            }
        }
        var updateTradeCommand = new TradeUpdateCommand
        {
            ReceiverId = trade.ReceiverId,
            Status = input.Status,
            TraderId = trade.TraderId,
            TradeDate = DateTime.UtcNow,
            TradeId = trade.TradeId,
            ReceiverArticleId = trade.ReceiverArticleId,
            TraderArticlesIds = trade.TraderArticlesIds,
        };
        
        _tradeCommandsProcessor.UpdateTrade(updateTradeCommand);

        return Ok(new { message = "Trade status and articles updated successfully.", newStatus = input.Status });
    }

    [HttpDelete("trades/{tradeId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteTrade(int tradeId)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        var tradeTmp = _tradesQueryProcessor.GetById(tradeId);
        if (tradeTmp.TraderId != userIdFromToken && tradeTmp.ReceiverId != userIdFromToken)
        {
            return Forbid();
        }
        
        try
        {
            _tradeCommandsProcessor.DeleteTrade(tradeId);
            return NoContent(); // Return 204
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Trade with ID {tradeId} not found."); // Return 404
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
}