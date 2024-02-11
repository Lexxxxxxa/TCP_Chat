using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Chat.Server
{
    class ChatClient
    {
        public TcpClient client;
        public NetworkStream stream;
        public string UserName { get; private set; }

        public ChatClient(TcpClient client, string userName)
        {
            this.client = client;
            this.stream = client.GetStream();
            this.UserName = userName;
        }

        public async Task StartReadAsync(List<ChatClient> clients, List<string> messageHistory)
        {
            var reader = new StreamReader(stream);
            try
            {
                while (true)
                {
                    var msg = await reader.ReadLineAsync();
                    if (msg == null) break;

                    var parts = msg.Split('|');
                    switch (parts[0])
                    {
                        case "PUBLIC":
                            var publicMessage = $"{UserName}: {parts[1]}";
                            Console.WriteLine($"Public message: {publicMessage}");
                            messageHistory.Add(publicMessage);
                            foreach (var client in clients.Where(c => c != this))
                            {
                                await client.SendMessage($"PUBLIC|{publicMessage}");
                            }
                            break;
                        case "PRIVATE":
                            var recipient = clients.FirstOrDefault(c => c.UserName == parts[1]);
                            if (recipient != null)
                            {
                                var privateMessage = $"Private from {UserName} to {parts[1]}: {parts[2]}";
                                Console.WriteLine(privateMessage);
                                await recipient.SendMessage($"PRIVATE|{UserName}: {parts[2]}");
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        public async Task SendMessage(string message)
        {
            var writer = new StreamWriter(stream) { AutoFlush = true };
            await writer.WriteLineAsync(message);
        }
    }
}
