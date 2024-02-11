using System;
using System.Net;
using System.Threading.Tasks;

namespace TCP_Chat.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Discovering servers...");
            var servers = await ServerDiscovery.DiscoverServersAsync();
            if (servers.Count == 0)
            {
                Console.WriteLine("No servers found. Exiting...");
                return;
            }

            var serverEndpoint = servers[0];
            Console.WriteLine($"Connecting to server {serverEndpoint}...");

            var chatClient = new ChatClient();
            await chatClient.ConnectAsync(serverEndpoint);
            await chatClient.RunAsync();
        }
    }
}
