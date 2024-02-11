using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TCP_Chat.Client
{
    public class ChatClient
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        public async Task ConnectAsync(IPEndPoint serverEndpoint)
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(serverEndpoint.Address, serverEndpoint.Port);
            Console.WriteLine("Connected!");

            stream = tcpClient.GetStream();
            writer = new StreamWriter(stream) { AutoFlush = true };
            reader = new StreamReader(stream);
        }

        public async Task RunAsync()
        {
            Console.Write("Enter your name: ");
            var userName = Console.ReadLine();
            Console.Write("Enter your password: ");
            var password = Console.ReadLine();

            await writer.WriteLineAsync($"AUTH|{userName}|{password}");

            var response = await reader.ReadLineAsync();
            if (response != "AUTH|SUCCESS")
            {
                Console.WriteLine("Authentication failed. Exiting...");
                return;
            }

            Console.WriteLine("Authenticated successfully.");

            var readTask = MessageReader.ReadMessagesAsync(reader);
            await MessageWriter.WriteMessagesAsync(writer, userName);
        }
    }
}
