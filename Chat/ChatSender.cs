using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    public class ChatSender
    {
        public async Task SendAsync(string ipString, string port, string message)
        {
            if (!int.TryParse(port, out int portNumber) ||
                !IPAddress.TryParse(ipString, out var address))
            {
                return;
            }

            var endpoint = new IPEndPoint(address, portNumber);
            var sender = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                await sender.ConnectAsync(endpoint);
                byte[] messageBytes = Encoding.ASCII.GetBytes(message + "<EOF>");
                var bytesSent = await sender.SendAsync(messageBytes, SocketFlags.None);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
        }
    }
}
