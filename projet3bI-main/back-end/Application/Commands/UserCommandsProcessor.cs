using Application.Commands.Create;
using Application.Commands.Delete;
using Application.Commands.update;
using Application.utils;
using Application.Utils;

namespace Application.Commands;

public class UserCommandsProcessor
{
    private readonly ICommandsHandler<BasicUserCreateCommand,UserCreateOutput> _createUserOutputHandler;
    private readonly ICommandsAdminHandler<UserCreateCommand,UserCreateOutput> _createAdminOutputHandler;
    private readonly IUserDeleteCommandHandler _deleteUserOutputHandler;
    private readonly IEmptyOutputCommandHandler<UserUpdateCommand> _updateUserOutputHandler;
    
    public UserCommandsProcessor(ICommandsHandler<BasicUserCreateCommand, UserCreateOutput> createUserOutputHandler,IUserDeleteCommandHandler deleteUserOutputHandler, IEmptyOutputCommandHandler<UserUpdateCommand> updateUserOutputHandler, ICommandsAdminHandler<UserCreateCommand, UserCreateOutput> createAdminOutputHandler)
    {
        _createUserOutputHandler = createUserOutputHandler;
        _deleteUserOutputHandler = deleteUserOutputHandler;
        _updateUserOutputHandler = updateUserOutputHandler;
        _createAdminOutputHandler = createAdminOutputHandler;
    }

    public UserCreateOutput CreateUser(BasicUserCreateCommand command)
    {
        return _createUserOutputHandler.Handle(command);
    }
    
    public UserCreateOutput CreateAdmin(UserCreateCommand command)
    {
        return _createAdminOutputHandler.HandleAdmin(command);
    }

    public void DeleteUser(int id)
    {
        _deleteUserOutputHandler.Handle(id);
    }

    public void UpdateUser(UserUpdateCommand command)
    {
        _updateUserOutputHandler.Handle(command);
    }
}