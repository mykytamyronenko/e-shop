using Application.Commands.Create;
using Application.Commands.Delete;
using Application.Commands.update;
using Application.utils;
using Application.Utils;

namespace Application.Commands;

public class UserMembershipCommandsProcessor
{
    private readonly ICommandsHandler<UserMembershipCreateCommand,UserMembershipCreateOutput> _createUserMembershipOutputHandler;
    private readonly IUserMembershipDeleteCommandHandler _deleteUserMembershipOutputHandler;
    private readonly IEmptyOutputCommandHandler<UserMembershipUpdateCommand> _updateUserMembershipOutputHandler;


    public UserMembershipCommandsProcessor(ICommandsHandler<UserMembershipCreateCommand, UserMembershipCreateOutput> createUserMembershipOutputHandler, IUserMembershipDeleteCommandHandler deleteUserMembershipOutputHandler, IEmptyOutputCommandHandler<UserMembershipUpdateCommand> updateUserMembershipOutputHandler)
    {
        _createUserMembershipOutputHandler = createUserMembershipOutputHandler;
        _deleteUserMembershipOutputHandler = deleteUserMembershipOutputHandler;
        _updateUserMembershipOutputHandler = updateUserMembershipOutputHandler;
    }

    public UserMembershipCreateOutput CreateUserMembership(UserMembershipCreateCommand command)
    {
        return _createUserMembershipOutputHandler.Handle(command);
    }

    public void DeleteUserMembership(int id)
    {
        _deleteUserMembershipOutputHandler.Handle(id);
    }

    public void UpdateUserMembership(UserMembershipUpdateCommand command)
    {
        _updateUserMembershipOutputHandler.Handle(command);
    }
}