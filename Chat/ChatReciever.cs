using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    public class ChatReceiver
    {
        private ConcurrentQueue<string> receivedMsgs;

        public ChatReceiver(ConcurrentQueue<string> receivedMsgs)
        {
            this.receivedMsgs = receivedMsgs;
        }

        public void StartReceiving(int port)
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);

            Socket receiver = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            byte[] buffer = new byte[1024];
            try
            {
                receiver.Bind(ipEndpoint);
                receiver.Listen(10);
                // Start listening for connections.  
                while (true)
                {
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = receiver.Accept();

                    string msg = null;

                    // Process incoming connection  
                    while (true)
                    {
                        int byteCount = handler.Receive(buffer);
                        msg += Encoding.ASCII.GetString(buffer, 0, byteCount);
                        int endIndex = msg.IndexOf("<EOF>");
                        if (endIndex > -1)
                        {
                            msg = msg.Substring(0, endIndex);
                            break;
                        }
                    }

                    receivedMsgs.Enqueue(msg);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
