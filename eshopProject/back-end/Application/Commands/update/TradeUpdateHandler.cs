using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Commands.update;

public class TradeUpdateHandler: IEmptyOutputCommandHandler<TradeUpdateCommand>
{
    private readonly ITradesRepository _tradesRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;

    public TradeUpdateHandler(ITradesRepository tradesRepository, IMapper mapper, TradeShopContext context)
    {
        _tradesRepository = tradesRepository;
        _mapper = mapper;
        _context = context;
    }

    public void Handle(in TradeUpdateCommand input)
    {
        using var transaction = _context.Database.BeginTransaction();
        var entity = _tradesRepository.GetById(input.TradeId)
                     ?? throw new TradeNotFoundException(input.TradeId);
        
        entity.TraderId = input.TraderId;
        entity.ReceiverId = input.ReceiverId;
        entity.TraderArticlesIds = input.TraderArticlesIds;
        entity.ReceiverArticleId = input.ReceiverArticleId;
        entity.TradeDate = input.TradeDate;
        entity.Status = input.Status;

        _tradesRepository.Update(entity);
        _context.SaveChanges();

        transaction.Commit();
    }
}