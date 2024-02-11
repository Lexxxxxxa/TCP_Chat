using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Chat.Server
{
    public class Program
    {
        static List<ChatClient> clients = new List<ChatClient>();
        static List<string> messageHistory = new List<string>();

        static async Task Main(string[] args)
        {
            var server = new TcpListener(IPAddress.Any, 7700);
            server.Start();
            var udpResponder = new UdpDiscoveryResponder(7701);
            var udpTask = udpResponder.StartRespondingAsync();

            Console.WriteLine("Server started.");

            try
            {
                while (true)
                {
                    var tcpClient = await server.AcceptTcpClientAsync();
                    var stream = tcpClient.GetStream();
                    var reader = new StreamReader(stream);
                    var authData = await reader.ReadLineAsync();
                    var parts = authData?.Split('|');
                    if (parts != null && parts.Length == 3 && parts[0] == "AUTH")
                    {
                        var userName = parts[1];
                        var password = parts[2];

                        var chatClient = new ChatClient(tcpClient, userName);
                        clients.Add(chatClient);
                        Console.WriteLine($"User {userName} connected.");

                        foreach (var message in messageHistory)
                        {
                            await chatClient.SendMessage($"HISTORY|{message}");
                        }

                        var clientTask = chatClient.StartReadAsync(clients, messageHistory);
                    }
                    else
                    {
                        var writer = new StreamWriter(stream) { AutoFlush = true };
                        await writer.WriteLineAsync("AUTH|FAIL");
                        tcpClient.Close();
                    }
                }
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Server stopped.");
            }
        }
    }
}
