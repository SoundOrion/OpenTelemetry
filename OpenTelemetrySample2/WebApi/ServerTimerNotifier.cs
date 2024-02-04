using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi;

public class ServerTimerNotifier : BackgroundService
{
    private readonly TimeSpan Period = TimeSpan.FromSeconds(5);
    private readonly ILogger<ServerTimerNotifier> _logger;
    private readonly IHubContext<NotificationHub, NotificationHubClient> _hubContext;

    public ServerTimerNotifier(ILogger<ServerTimerNotifier> logger, IHubContext<NotificationHub, NotificationHubClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var Timer = new PeriodicTimer(Period);

        while (!stoppingToken.IsCancellationRequested &&
            await Timer.WaitForNextTickAsync(stoppingToken))
        {
            var dateTime = DateTime.Now;

            _logger.LogInformation("Executing {Service} {Time}", nameof(ServerTimerNotifier), dateTime);

            //await _hubContext.Clients.All.ReceiveNotification($"Server time = {dateTime}");
            await _hubContext.Clients
                .User("anonymous")
                .ReceiveNotification($"Server time = {dateTime}");
        }
    }
}
