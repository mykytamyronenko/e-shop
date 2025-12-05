using Application.Commands.Create;
using Application.Commands.Delete;
using Application.Commands.update;
using Application.utils;
using Application.Utils;

namespace Application.Commands;

public class TradeCommandsProcessor
{
    private readonly ICommandsHandler<TradeCreateCommand,TradeCreateOutput> _createTradeOutputHandler;
    private readonly ITradeDeleteCommandHandler _deleteTradeOutputHandler;
    private readonly IEmptyOutputCommandHandler<TradeUpdateCommand> _updateTradeOutputHandler;


    public TradeCommandsProcessor(ICommandsHandler<TradeCreateCommand, TradeCreateOutput> createTradeOutputHandler, ITradeDeleteCommandHandler deleteTradeOutputHandler, IEmptyOutputCommandHandler<TradeUpdateCommand> updateTradeOutputHandler)
    {
        _createTradeOutputHandler = createTradeOutputHandler;
        _deleteTradeOutputHandler = deleteTradeOutputHandler;
        _updateTradeOutputHandler = updateTradeOutputHandler;
    }

    public TradeCreateOutput CreateTrade(TradeCreateCommand command)
    {
        return _createTradeOutputHandler.Handle(command);
    }

    public void DeleteTrade(int id)
    {
        _deleteTradeOutputHandler.Handle(id);
    }

    public void UpdateTrade(TradeUpdateCommand command)
    {
        _updateTradeOutputHandler.Handle(command);
    }
}