using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.Getall;

public class ArticlesGetAllHandler : IQueryHandler<ArticlesGetAllQuery,ArticlesGetAllOutput>
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly IMapper _mapper;

    public ArticlesGetAllHandler(IArticlesRepository articlesRepository, IMapper mapper)
    {
        _articlesRepository = articlesRepository;
        _mapper = mapper;
    }


    public ArticlesGetAllOutput Handle(ArticlesGetAllQuery query)
    {
        var dbArticles = _articlesRepository.GetAll();
        var output = new ArticlesGetAllOutput()
        {
           ArticlesList  = _mapper.Map<List<ArticlesGetAllOutput.Articles>>(dbArticles)
        };
        return output;
    }
}