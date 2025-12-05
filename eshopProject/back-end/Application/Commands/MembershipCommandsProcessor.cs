using Application.Commands.Create;
using Application.Commands.Delete;
using Application.Commands.update;
using Application.utils;
using Application.Utils;

namespace Application.Commands;

public class MembershipCommandsProcessor
{
    private readonly ICommandsHandler<MembershipCreateCommand,MembershipCreateOutput> _createMembershipOutputHandler;
    private readonly IMembershipDeleteCommandHandler _deleteMembershipCommandHandler;
    private readonly IEmptyOutputCommandHandler<MembershipUpdateCommand> _updateMembershipCommandHandler;


    public MembershipCommandsProcessor(ICommandsHandler<MembershipCreateCommand, MembershipCreateOutput> createMembershipOutputHandler, IMembershipDeleteCommandHandler deleteMembershipCommandHandler, IEmptyOutputCommandHandler<MembershipUpdateCommand> updateMembershipCommandHandler)
    {
        _createMembershipOutputHandler = createMembershipOutputHandler;
        _deleteMembershipCommandHandler = deleteMembershipCommandHandler;
        _updateMembershipCommandHandler = updateMembershipCommandHandler;
    }

    public MembershipCreateOutput CreateMembership(MembershipCreateCommand command)
    {
        return _createMembershipOutputHandler.Handle(command);
    }

    public void DeleteMembership(int id)
    {
        _deleteMembershipCommandHandler.Handle(id);
    }

    public void UpdateMembership(MembershipUpdateCommand command)
    {
        _updateMembershipCommandHandler.Handle(command);
    }
}