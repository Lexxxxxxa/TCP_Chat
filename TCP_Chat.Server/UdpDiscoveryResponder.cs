using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Chat.Server
{
    public class UdpDiscoveryResponder
    {
        private readonly UdpClient udpClient;
        private readonly int listenPort;

        public UdpDiscoveryResponder(int listenPort)
        {
            this.listenPort = listenPort;
            udpClient = new UdpClient(listenPort);
            udpClient.EnableBroadcast = true;
        }

        public async Task StartRespondingAsync()
        {
            Console.WriteLine($"UDP discovery responder started on port {listenPort}.");

            while (true)
            {
                var receivedResult = await udpClient.ReceiveAsync();
                var message = Encoding.UTF8.GetString(receivedResult.Buffer);

                if (message == "DISCOVER_CHAT_SERVER_REQUEST")
                {
                    var responseBytes = Encoding.UTF8.GetBytes("DISCOVER_CHAT_SERVER_RESPONSE|" + receivedResult.RemoteEndPoint.Address.ToString());
                    await udpClient.SendAsync(responseBytes, responseBytes.Length, receivedResult.RemoteEndPoint);
                }
            }
        }
    }
}
