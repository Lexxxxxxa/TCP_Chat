using System;
using System.IO;
using System.Threading.Tasks;

namespace TCP_Chat.Client
{
    public static class MessageReader
    {
        public static async Task ReadMessagesAsync(StreamReader reader)
        {
            while (true)
            {
                var message = await reader.ReadLineAsync();
                if (message?.StartsWith("PRIVATE") == true)
                {
                    Console.WriteLine($"Private message: {message.Substring(7)}");
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }
    }
}
