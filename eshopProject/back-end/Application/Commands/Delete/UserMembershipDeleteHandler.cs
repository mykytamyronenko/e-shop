using Application.utils;
using Infrastructure;

namespace Application.Commands.Delete;

public class UserMembershipDeleteHandler : IUserMembershipDeleteCommandHandler
{
    private readonly IUserMembershipsRepository _userMembershipsRepository;
    private readonly TradeShopContext _context;
    
    public UserMembershipDeleteHandler(IUserMembershipsRepository userMembershipsRepository, TradeShopContext context)
    {
        _userMembershipsRepository = userMembershipsRepository;
        _context = context;
    }

    public void Handle(in int id)
    {
        if (_userMembershipsRepository.GetById(id) is not null)
        {
            _userMembershipsRepository.Delete(id);
            _context.SaveChanges();
        }
        else
        {
            throw new InvalidOperationException("User membership not found");
        }
    }

}