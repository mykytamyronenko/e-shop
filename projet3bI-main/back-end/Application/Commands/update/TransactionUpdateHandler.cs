using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Commands.update;

public class TransactionUpdateHandler:IEmptyOutputCommandHandler<TransactionUpdateCommand>
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;

    public TransactionUpdateHandler(ITransactionsRepository transactionsRepository, IMapper mapper, TradeShopContext context)
    {
        _transactionsRepository = transactionsRepository;
        _mapper = mapper;
        _context = context;
    }

    public void Handle(in TransactionUpdateCommand input)
    {
        using var transaction = _context.Database.BeginTransaction();
        var entity = _transactionsRepository.GetById(input.TransactionId)
                     ?? throw new TransactionNotFoundException(input.TransactionId);
        
        entity.BuyerId = input.BuyerId;
        entity.SellerId = input.SellerId;
        entity.ArticleId = input.ArticleId;
        entity.TransactionType = input.TransactionType;
        entity.Price = input.Price;
        entity.Commission = input.Commission;
        entity.TransactionDate = input.TransactionDate;
        entity.Status = input.Status;
        
        _transactionsRepository.Update(entity);
        _context.SaveChanges();
        
        transaction.Commit();
    }
}