using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api")]
public class ArticleQueryController: ControllerBase
{
    private readonly ArticlesQueryProcessor _articlesQueryProcessor;

    public ArticleQueryController(ArticlesQueryProcessor articlesQueryProcessor)
    {
        _articlesQueryProcessor = articlesQueryProcessor;
    }
    
    [HttpGet("articles")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public List<ArticlesGetAllOutput.Articles> GetAllArticles()
    {
        return _articlesQueryProcessor.GetAll(null!).ArticlesList;
    }


    [HttpGet("articles/{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ArticlesGetByIdOutput> GetByIdArticle(int id)
    {
        if (id < 0)
        {
            return BadRequest("The article ID must be more than zero."); // Retourne 400
        }

        try
        {
            var articleOutput = _articlesQueryProcessor.GetById(id);
            return Ok(articleOutput); // Retourne 200 avec les données de l'utilisateur
        }
        catch (ArticleNotFoundException ex)
        {
            return NotFound(ex.Message); // Retourne 404 avec le message de l'exception
        }

    }
    
    [HttpGet("articles/getTitle")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult GetTitleById(int id)
    {
        try
        {
            var articleOutput = _articlesQueryProcessor.GetById(id);
            var title = articleOutput.Title;
            return Ok(new { title = title });
        }
        catch (ArticleNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
}