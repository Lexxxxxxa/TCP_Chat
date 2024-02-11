using System;
using System.IO;
using System.Threading.Tasks;

namespace TCP_Chat.Client
{
    public static class MessageWriter
    {
        public static async Task WriteMessagesAsync(StreamWriter writer, string userName)
        {
            while (true)
            {
                var message = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (message.StartsWith("/private"))
                    {
                        var parts = message.Split(' ', 3);
                        if (parts.Length == 3)
                        {
                            await writer.WriteLineAsync($"PRIVATE|{parts[1]}|{parts[2]}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid private message format. Use '/private [recipient] [message]'.");
                        }
                    }
                    else
                    {
                        await writer.WriteLineAsync($"PUBLIC|{message}");
                    }
                }
            }
        }
    }
}
