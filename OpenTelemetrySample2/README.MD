# コードの参考
Real-Time Notifications Using Blazor and SignalR from scratch
https://www.youtube.com/watch?v=O7oaxFgNuYo

# Jwt
このサンプルコードでは、ユーザー名"anonymous"に対してトークンを発行した

https://aadojo.alterbooth.com/entry/2022/12/03/100000
.csproj のディレクトリで dotent user-jwts create を実行すると JWT が発行されます。
dotnet user-jwts create
dotnet user-jwts create -n "anonymous"
↑
こうすると名前設定できる、引数なしだと環境変数のものが使われるらしい

https://jwt.io/



認証通らないとアクセスできない
[Authorize]
public class NotificationHub : Hub<NotificationHubClient>


"anonymous"にだけメッセージが行く
            await _hubContext.Clients
                .User("anonymous")
                .ReceiveNotification($"Server time = {dateTime}");

"anonymous"に対して発行したトークン貼り付けているので、"anonymous"宛のメッセージ受信可能
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7246/notifications",
            o => o.AccessTokenProvider = () => Task.FromResult<string?>("eyJhbG...