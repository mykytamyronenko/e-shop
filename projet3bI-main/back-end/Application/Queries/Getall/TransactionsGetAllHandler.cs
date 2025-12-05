using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.Getall;

public class TransactionsGetAllHandler: IQueryHandler<TransactionsGetAllQuery, TransactionsGetAllOutput>
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IMapper _mapper;

    public TransactionsGetAllHandler(ITransactionsRepository transactionsRepository, IMapper mapper)
    {
        _transactionsRepository = transactionsRepository;
        _mapper = mapper;
    }

    public TransactionsGetAllOutput Handle(TransactionsGetAllQuery query)
    {
        var dbTransactions = _transactionsRepository.GetAll();
        var output = new TransactionsGetAllOutput()
        {
            TransactionsList = _mapper.Map<List<TransactionsGetAllOutput.Transactions>>(dbTransactions)
        };
        
        return output;
    }
}
