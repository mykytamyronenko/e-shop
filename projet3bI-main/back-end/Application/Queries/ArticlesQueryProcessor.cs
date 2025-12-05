using Application.Queries.Getall;
using Application.Queries.getById;
using Application.utils;

namespace Application.Queries;

public class ArticlesQueryProcessor
{
    private readonly IQueryHandler<ArticlesGetAllQuery,ArticlesGetAllOutput> _articleQueryHandler;
    private readonly IQueryHandler<int, ArticlesGetByIdOutput> _articleQueryHandlerId;

    public ArticlesQueryProcessor(IQueryHandler<int, ArticlesGetByIdOutput> articleQueryHandlerId, IQueryHandler<ArticlesGetAllQuery, ArticlesGetAllOutput> articleQueryHandler)
    {
        _articleQueryHandlerId = articleQueryHandlerId;
        _articleQueryHandler = articleQueryHandler;
    }


    public ArticlesGetAllOutput GetAll(ArticlesGetAllQuery query)
    {
        return _articleQueryHandler.Handle(query);
    }
    public ArticlesGetByIdOutput GetById(int id)
    {
        return _articleQueryHandlerId.Handle(id);
    }

}