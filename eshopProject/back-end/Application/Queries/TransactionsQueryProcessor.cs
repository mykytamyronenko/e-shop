using Application.Queries.Getall;
using Application.Queries.getById;
using Application.utils;

namespace Application.Queries;

public class TransactionsQueryProcessor
{
    private readonly IQueryHandler<TransactionsGetAllQuery, TransactionsGetAllOutput> _transactionQueryHandler;
    private readonly IQueryHandler<int, TransactionsGetByIdOutput> _transactionQueryHandlerId;
    
    public TransactionsQueryProcessor(IQueryHandler<TransactionsGetAllQuery, TransactionsGetAllOutput> transactionQueryHandler, IQueryHandler<int, TransactionsGetByIdOutput> transactionQueryHandlerId)
    {
        _transactionQueryHandler = transactionQueryHandler;
        _transactionQueryHandlerId = transactionQueryHandlerId;
    }

    public TransactionsGetAllOutput GetAll(TransactionsGetAllQuery query)
    {
        return _transactionQueryHandler.Handle(query);
    }
    
    public TransactionsGetByIdOutput GetById(int id)
    {
        return _transactionQueryHandlerId.Handle(id);
    }
}