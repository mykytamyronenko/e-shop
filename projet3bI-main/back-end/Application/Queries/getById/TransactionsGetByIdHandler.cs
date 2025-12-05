using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.getById;

public class TransactionsGetByIdHandler : IQueryHandler<int,TransactionsGetByIdOutput>
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IMapper _mapper;


    public TransactionsGetByIdHandler(ITransactionsRepository transactionsRepository, IMapper mapper)
    {
        _transactionsRepository = transactionsRepository;
        _mapper = mapper;
    }

    public TransactionsGetByIdOutput Handle(int id)
    {
        var dbTransaction = _transactionsRepository.GetById(id) ?? throw new TransactionNotFoundException(id);

        return _mapper.Map<TransactionsGetByIdOutput>(dbTransaction);
    }
}