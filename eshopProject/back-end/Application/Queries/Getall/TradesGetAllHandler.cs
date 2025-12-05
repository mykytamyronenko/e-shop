using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.Getall;

public class TradesGetAllHandler : IQueryHandler<TradesGetAllQuery, TradesGetAllOutput>
{
    private readonly ITradesRepository _tradeRepository;
    private readonly IMapper _mapper;

    public TradesGetAllHandler(ITradesRepository tradeRepository, IMapper mapper)
    {
        _tradeRepository = tradeRepository;
        _mapper = mapper;
    }


    public TradesGetAllOutput Handle(TradesGetAllQuery query)
    {
        var dbTrades = _tradeRepository.GetAll();
        var output = new TradesGetAllOutput()
        {
            TradesList = _mapper.Map<List<TradesGetAllOutput.Trades>>(dbTrades)
        };

        return output;
    }
}