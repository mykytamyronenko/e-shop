using Application.Commands.Create;
using Application.Commands.Delete;
using Application.Commands.update;
using Application.utils;
using Application.Utils;

namespace Application.Commands;

public class RatingCommandsProcessor
{
    private readonly ICommandsHandler<RatingCreateCommand,RatingCreateOutput> _createRatingOutputHandler;
    private readonly IRatingDeleteCommandHandler _deleteRatingOutputHandler;
    private readonly IEmptyOutputCommandHandler<RatingUpdateCommand> _updateRatingOutputHandler;


    public RatingCommandsProcessor(ICommandsHandler<RatingCreateCommand, RatingCreateOutput> createRatingOutputHandler, IRatingDeleteCommandHandler deleteRatingOutputHandler, IEmptyOutputCommandHandler<RatingUpdateCommand> updateRatingOutputHandler)
    {
        _createRatingOutputHandler = createRatingOutputHandler;
        _deleteRatingOutputHandler = deleteRatingOutputHandler;
        _updateRatingOutputHandler = updateRatingOutputHandler;
    }

    public RatingCreateOutput CreateRating(RatingCreateCommand command)
    {
        return _createRatingOutputHandler.Handle(command);
    }

    public void DeleteRating(int id)
    {
        _deleteRatingOutputHandler.Handle(id);
    }

    public void UpdateRating(RatingUpdateCommand command)
    {
        _updateRatingOutputHandler.Handle(command);
    }
}