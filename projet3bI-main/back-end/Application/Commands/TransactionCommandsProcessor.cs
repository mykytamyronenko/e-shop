using Application.Commands.Create;
using Application.Commands.Delete;
using Application.Commands.update;
using Application.utils;
using Application.Utils;

namespace Application.Commands;

public class TransactionCommandsProcessor
{
    private readonly ICommandsHandler<TransactionCreateCommand,TransactionCreateOutput> _createTransactionOutputHandler;
    private readonly ITransactionDeleteCommandHandler _deleteTransactionOutputHandler;
    private readonly IEmptyOutputCommandHandler<TransactionUpdateCommand> _updateTransactionOutputHandler;


    public TransactionCommandsProcessor(ICommandsHandler<TransactionCreateCommand, TransactionCreateOutput> createTransactionOutputHandler, ITransactionDeleteCommandHandler deleteTransactionOutputHandler, IEmptyOutputCommandHandler<TransactionUpdateCommand> updateTransactionOutputHandler)
    {
        _createTransactionOutputHandler = createTransactionOutputHandler;
        _deleteTransactionOutputHandler = deleteTransactionOutputHandler;
        _updateTransactionOutputHandler = updateTransactionOutputHandler;
    }

    public TransactionCreateOutput CreateTransaction(TransactionCreateCommand command)
    {
        return _createTransactionOutputHandler.Handle(command);
    }

    public void DeleteTransaction(int id)
    {
        _deleteTransactionOutputHandler.Handle(id);
    }

    public void UpdateTransaction(TransactionUpdateCommand command)
    {
        _updateTransactionOutputHandler.Handle(command);
    }
}