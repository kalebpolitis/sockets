using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    public class ChatSender
    {
        public void Send(string ipString, string port, string message)
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
                sender.Connect(endpoint);
                byte[] messageBytes = Encoding.ASCII.GetBytes(message + "<EOF>");
                var bytesSent = sender.Send(messageBytes);
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
