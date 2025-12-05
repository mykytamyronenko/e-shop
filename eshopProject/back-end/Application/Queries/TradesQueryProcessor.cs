using Application.Queries.Getall;
using Application.Queries.getById;
using Application.utils;

namespace Application.Queries;

public class TradesQueryProcessor
{
    private readonly IQueryHandler<TradesGetAllQuery,TradesGetAllOutput> _tradeQueryHandler;
    private readonly IQueryHandler<int, TradesGetByIdOutput> _tradeQueryHandlerId;

    public TradesQueryProcessor(IQueryHandler<TradesGetAllQuery, TradesGetAllOutput> tradeQueryHandler, IQueryHandler<int, TradesGetByIdOutput> tradeQueryHandlerId)
    {
        _tradeQueryHandler = tradeQueryHandler;
        _tradeQueryHandlerId = tradeQueryHandlerId;
    }

    public TradesGetAllOutput GetAll(TradesGetAllQuery query)
    {
        return _tradeQueryHandler.Handle(query);
    }

    public TradesGetByIdOutput GetById(int id)
    {
        return _tradeQueryHandlerId.Handle(id);
    }

}