using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs;

[Authorize]
public class NotificationHub : Hub<NotificationHubClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Client(Context.ConnectionId).ReceiveNotification(
            $"Thank you for connecting {Context.User?.Identity?.Name}");

        await base.OnConnectedAsync();
    }
}

public interface NotificationHubClient
{
    Task ReceiveNotification(string message);
}