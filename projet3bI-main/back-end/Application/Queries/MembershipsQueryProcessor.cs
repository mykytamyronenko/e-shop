using Application.Queries.Getall;
using Application.Queries.getById;
using Application.utils;

namespace Application.Queries;

public class MembershipsQueryProcessor
{
    private readonly IQueryHandler<MembershipGetAllQuery,MembershipGetAllOutput> _membershipQueryHandler;
    private readonly IQueryHandler<int, MembershipsGetByIdOutput> _membershipQueryHandlerId;


    public MembershipsQueryProcessor(IQueryHandler<MembershipGetAllQuery, MembershipGetAllOutput> membershipQueryHandler, IQueryHandler<int, MembershipsGetByIdOutput> membershipQueryHandlerId)
    {
        _membershipQueryHandler = membershipQueryHandler;
        _membershipQueryHandlerId = membershipQueryHandlerId;
    }

    public MembershipGetAllOutput GetAll(MembershipGetAllQuery query)
    {
        return _membershipQueryHandler.Handle(query);
    }
    public MembershipsGetByIdOutput GetById(int id)
    {
        return _membershipQueryHandlerId.Handle(id);
    }

}