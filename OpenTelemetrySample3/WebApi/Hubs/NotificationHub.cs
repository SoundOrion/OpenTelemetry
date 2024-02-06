using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs;

public class NotificationHub : Hub<NotificationHubClient>
{
}

public interface NotificationHubClient
{
    Task ReceiveNotification(string message);
}