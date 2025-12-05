using System.Security.Cryptography;
using System.Text;
using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.update;

public class UserMembershipUpdateHandler : IEmptyOutputCommandHandler<UserMembershipUpdateCommand>
{
    private readonly IUserMembershipsRepository _userMembershipsRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;
    
    public UserMembershipUpdateHandler(IUserMembershipsRepository userMembershipsRepository, IMapper mapper, TradeShopContext context)
    {
        _userMembershipsRepository = userMembershipsRepository;
        _mapper = mapper;
        _context = context;
    }

    public void Handle(in UserMembershipUpdateCommand input)
    {
        using var transaction = _context.Database.BeginTransaction();
        var entity = _userMembershipsRepository.GetById(input.UserMembershipId)
                     ?? throw new UserMembershipNotFoundException(input.UserMembershipId);
        
        entity.UserId = input.UserId;
        entity.MembershipId = input.MembershipId;
        entity.StartDate = input.StartDate;
        entity.EndDate = input.EndDate;
        entity.Status = input.Status;
        
        _userMembershipsRepository.Update(entity);
        _context.SaveChanges();

        transaction.Commit();
    }
    
}