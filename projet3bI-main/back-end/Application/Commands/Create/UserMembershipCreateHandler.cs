using Application.exceptions;
using Application.Utils;
using AutoMapper;
using Domain;
using Infrastructure;

namespace Application.Commands.Create;

public class UserMembershipCreateHandler: ICommandsHandler<UserMembershipCreateCommand,UserMembershipCreateOutput>
{
    private readonly IUserMembershipsRepository _userMembershipsRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;
    
    public UserMembershipCreateHandler(IUserMembershipsRepository userMembershipsRepository, IMapper mapper, TradeShopContext context)
    {
        _userMembershipsRepository = userMembershipsRepository;
        _mapper = mapper;
        _context = context;
    }

    public UserMembershipCreateOutput Handle(UserMembershipCreateCommand input) {
        var allowedStatuses = new[] { "active", "suspended", "deleted" };
        if (!allowedStatuses.Contains(input.Status))
        {
            throw new ArgumentException("Invalid status. Allowed values are 'active', 'suspended', and 'deleted'.");
        }
        
        using var transaction = _context.Database.BeginTransaction();
        
        var buyer = _context.Users.FirstOrDefault(u => u.UserId == input.UserId)
            ?? throw new UserNotFoundException(input.UserId);
        
        var membership = _context.Memberships.FirstOrDefault(m => m.MembershipId == input.MembershipId)
            ?? throw new MembershipNotFoundException(input.MembershipId);

        if (buyer.Balance < membership.Price)
        {
            throw new InvalidOperationException("User does not have enough balance to purchase the membership.");
        }
        
        buyer.Balance -= membership.Price;
        
        var userMembership = new UserMemberships
        {
            UserId = input.UserId,
            MembershipId = input.MembershipId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30),
            Status = string.IsNullOrEmpty(input.Status) ? "active" : input.Status
        };
 
        
        var existingUserMembership = _context.UserMemberships
            .FirstOrDefault(a => a.UserMembershipId == userMembership.UserMembershipId);

        if (existingUserMembership != null)
        {
            throw new Exception("User Membership already exists.");
        }
        
        var existingUserInMembership = _context.UserMemberships
            .FirstOrDefault(m => m.UserId == userMembership.UserId);

        if (existingUserInMembership != null)
        {
            throw new InvalidOperationException($"This user is already subscribe to a membership.");
        }
        
        _context.Users.Update(buyer);
        _userMembershipsRepository.Create(userMembership);
        _context.SaveChanges();
        
        transaction.Commit();
        
        return _mapper.Map<UserMembershipCreateOutput>(userMembership);
    }
}