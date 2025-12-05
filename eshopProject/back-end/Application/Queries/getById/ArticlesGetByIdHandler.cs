using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.getById;

public class ArticlesGetByIdHandler : IQueryHandler<int,ArticlesGetByIdOutput>
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly IMapper _mapper;


    public ArticlesGetByIdHandler(IArticlesRepository articlesRepository, IMapper mapper)
    {
        _articlesRepository = articlesRepository;
        _mapper = mapper;
    }

    public ArticlesGetByIdOutput Handle(int id)
    {
        var dbArticle = _articlesRepository.GetById(id) ?? throw new ArticleNotFoundException(id);

        return _mapper.Map<ArticlesGetByIdOutput>(dbArticle);
    }


    
}