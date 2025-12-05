using Application.Queries.Getall;
using Application.Queries.getById;
using Application.utils;

namespace Application.Queries;

public class UserMembershipsQueryProcessor
{
    private readonly IQueryHandler<UsersMembershipGetAllQuery,UsersMembershipGetAllOutput> _userMembershipsQueryHandler;
    private readonly IQueryHandler<int, UserMembershipsGetByIdOutput> _userMembershipQueryHandlerId;


    public UserMembershipsQueryProcessor(IQueryHandler<UsersMembershipGetAllQuery, UsersMembershipGetAllOutput> userMembershipsQueryHandler, IQueryHandler<int, UserMembershipsGetByIdOutput> userMembershipQueryHandlerId)
    {
        _userMembershipsQueryHandler = userMembershipsQueryHandler;
        _userMembershipQueryHandlerId = userMembershipQueryHandlerId;
    }

    public UsersMembershipGetAllOutput GetAll(UsersMembershipGetAllQuery query)
    {
        return _userMembershipsQueryHandler.Handle(query);
    }
    public UserMembershipsGetByIdOutput GetById(int id)
    {
        return _userMembershipQueryHandlerId.Handle(id);
    }

}