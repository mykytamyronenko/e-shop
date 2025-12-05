using Infrastructure;

namespace Application.Services;

public class UserMembershipCleanupService
{
    private readonly IUserMembershipsRepository _userMembershipsRepository;
    private readonly TradeShopContext _context;

    public UserMembershipCleanupService(IUserMembershipsRepository userMembershipsRepository, TradeShopContext context)
    {
        _userMembershipsRepository = userMembershipsRepository;
        _context = context;
    }

    public void CleanupExpiredMemberships()
    {
        var now = DateTime.Now;
        var expiredMemberships = _userMembershipsRepository.GetExpiredMemberships(now);

        foreach (var subscription in expiredMemberships)
        {
            _userMembershipsRepository.Delete(subscription.UserMembershipId);
        }

        _context.SaveChanges();
    }
}