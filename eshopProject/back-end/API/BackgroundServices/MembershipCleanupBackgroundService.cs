using Application.Services;

namespace API.BackgroundServices;

public class MembershipCleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public MembershipCleanupBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cleanupService = scope.ServiceProvider.GetRequiredService<UserMembershipCleanupService>();
                cleanupService.CleanupExpiredMemberships();
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}