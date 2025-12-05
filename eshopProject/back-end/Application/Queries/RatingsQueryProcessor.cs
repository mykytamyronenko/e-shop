using Application.Queries.Getall;
using Application.Queries.getById;
using Application.utils;

namespace Application.Queries;

public class RatingsQueryProcessor
{
    private readonly IQueryHandler<RatingsGetAllQuery,RatingsGetAllOutput> _ratingQueryHandler;
    private readonly IQueryHandler<int, RatingsGetByIdOutput> _ratingQueryHandlerId;

    public RatingsQueryProcessor(IQueryHandler<RatingsGetAllQuery, RatingsGetAllOutput> ratingQueryHandler, IQueryHandler<int, RatingsGetByIdOutput> ratingQueryHandlerId)
    {
        _ratingQueryHandler = ratingQueryHandler;
        _ratingQueryHandlerId = ratingQueryHandlerId;
    }
    
    public RatingsGetAllOutput GetAll(RatingsGetAllQuery query)
    {
        return _ratingQueryHandler.Handle(query);
    }
    
    public RatingsGetByIdOutput GetById(int id)
    {
        return _ratingQueryHandlerId.Handle(id);
    }
}