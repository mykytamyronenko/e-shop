using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.getById;

public class TradesGetByIdHandler: IQueryHandler<int,TradesGetByIdOutput>
{
    private readonly ITradesRepository _tradesRepository;
    private readonly IMapper _mapper;


    public TradesGetByIdHandler(ITradesRepository tradesRepository, IMapper mapper)
    {
        _tradesRepository = tradesRepository;
        _mapper = mapper;
    }

    public TradesGetByIdOutput Handle(int id)
    {
        var dbTrade = _tradesRepository.GetById(id) ?? throw new TradeNotFoundException(id);

        return _mapper.Map<TradesGetByIdOutput>(dbTrade);
    }
}