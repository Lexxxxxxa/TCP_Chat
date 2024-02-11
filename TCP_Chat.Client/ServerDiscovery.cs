using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Chat.Client
{
    public static class ServerDiscovery
    {
        public static async Task<List<IPEndPoint>> DiscoverServersAsync()
        {
            var udpClient = new UdpClient { EnableBroadcast = true };
            var endpoint = new IPEndPoint(IPAddress.Broadcast, 7701);
            await udpClient.SendAsync(Encoding.UTF8.GetBytes("DISCOVER_CHAT_SERVER_REQUEST"), endpoint);
            var servers = new List<IPEndPoint>();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (var cancellationTokenSource = new CancellationTokenSource(5000))
            {
                cancellationTokenSource.Token.Register(() => taskCompletionSource.TrySetResult(true));
                while (!taskCompletionSource.Task.IsCompleted)
                {
                    var result = await udpClient.ReceiveAsync();
                    var message = Encoding.UTF8.GetString(result.Buffer);
                    if (message.StartsWith("DISCOVER_CHAT_SERVER_RESPONSE"))
                    {
                        servers.Add(result.RemoteEndPoint);
                        Console.WriteLine($"Found server: {result.RemoteEndPoint}");
                    }
                }
            }

            return servers;
        }
    }
}
