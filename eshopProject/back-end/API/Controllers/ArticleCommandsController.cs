using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class ArticleCommandsController:ControllerBase
{
    private readonly ArticleCommandsProcessor _articleCommandsProcessor;
    private readonly ArticlesQueryProcessor _articlesQueryProcessor;

    public ArticleCommandsController(ArticleCommandsProcessor articleCommandsProcessor, ArticlesQueryProcessor articlesQueryProcessor)
    {
        _articleCommandsProcessor = articleCommandsProcessor;
        _articlesQueryProcessor = articlesQueryProcessor;
    }
    
    [HttpPost("articles")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<ArticleCreateOutput> CreateArticle([FromForm] ArticleCreateCommand command)
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
            return BadRequest("Invalid article data."); // Return 400
        }
        try
        {
            var result = _articleCommandsProcessor.CreateArticle(command);
            return Ok(result); // Return 200
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }    
    }
    
    [HttpPut("articles/{articleId}")]
    [Authorize]
    public IActionResult UpdateArticle(ArticleUpdateCommand command)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }

        if (_articlesQueryProcessor.GetById(command.ArticleId).UserId != userIdFromToken)
        {
            return Unauthorized("You can't update an article you don't own");
        }
        
        if (command.UserId != userIdFromToken)
        {
            return Forbid();
        }
        
        _articleCommandsProcessor.UpdateArticle(command);
        return Ok(new { message = "Article updated." });
    }
    
    [HttpDelete("articles/{articleId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteArticle(int articleId)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        var articleTmp = _articlesQueryProcessor.GetById(articleId);
        if (articleTmp.UserId != userIdFromToken)
        {
            return Forbid();
        }
        
        try
        {
            _articleCommandsProcessor.DeleteArticle(articleId);
            return NoContent(); // Return 204
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Article with ID {articleId} not found."); // Return 404
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Return 500
        }
    }
    
    [HttpPut("articles/{articleId}/status")]
    [Authorize]
    public IActionResult UpdateArticleStatus(int articleId, [FromBody] ArticleUpdateCommandStatus input)
    {
        if (!int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userIdFromToken))
        {
            return Unauthorized("Invalid token: User ID not found.");
        }
        
        var article = _articlesQueryProcessor.GetById(articleId);
        if (article == null)
        {
            return NotFound("Article not found.");
        }
        
        if (article.UserId != userIdFromToken)
        {
            return Forbid("You do not have permission to remove this article.");
        }

        var updateArticleCommand = new ArticleUpdateCommand
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Description = article.Description,
            UserId = article.UserId,
            Price = article.Price,
            CreatedAt = article.CreatedAt,
            UpdatedAt = DateTime.Now,
            Category = article.Category,
            Quantity = article.Quantity,
            State = article.State,
            Status = "removed",
            MainImageUrl = article.MainImageUrl
        };
        
        _articleCommandsProcessor.UpdateArticle(updateArticleCommand);

        return Ok(new { message = "Article removed successfully.", newStatus = input.Status });
    }
    
    [HttpPut("articles/{articleId}/admin/status")]
    [Authorize(Roles = "admin")]
    public IActionResult UpdateArticleStatusAdmin(int articleId, [FromBody] ArticleUpdateCommandStatus input)
    {
        var article = _articlesQueryProcessor.GetById(articleId);
        if (article == null)
        {
            return NotFound("Article not found.");
        }

        var updateArticleCommand = new ArticleUpdateCommand
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Description = article.Description,
            UserId = article.UserId,
            Price = article.Price,
            CreatedAt = article.CreatedAt,
            UpdatedAt = DateTime.Now,
            Category = article.Category,
            Quantity = article.Quantity,
            State = article.State,
            Status = "removed",
            MainImageUrl = article.MainImageUrl
        };
        
        _articleCommandsProcessor.UpdateArticle(updateArticleCommand);

        return Ok(new { message = "Article removed successfully.", newStatus = input.Status });
    }
}