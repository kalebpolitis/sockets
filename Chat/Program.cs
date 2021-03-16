using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            ushort portNumber = 0;
            var arguments = GetArguments(args);
            if (arguments.TryGetValue("port", out string port))
            {
                if (ushort.TryParse(port, out portNumber))
                {
                    Console.WriteLine($"Listening to port {portNumber}.");
                }
                else
                {
                    Console.WriteLine($"Invalid port: {port}.");
                    Environment.Exit(-1);
                }
            }
            else
            {
                Console.WriteLine("No port provided.");
                Environment.Exit(-1);
            }

            ConcurrentQueue<string> receivedMsgs = new ConcurrentQueue<string>();
            ChatReceiver receiver = new ChatReceiver(receivedMsgs);
            Task.Run(() => receiver.StartReceiving(portNumber));

            ChatSender sender = new ChatSender();
            ConcurrentQueue<string> toSendMsgs = new ConcurrentQueue<string>();
            Task.Run(() => EnqueueToSend(toSendMsgs));
        
            while (true)
            {
                if (receivedMsgs.Count > 0)
                {
                    while (receivedMsgs.TryDequeue(out string receivedMsg))
                    {
                        Console.WriteLine($"Received: {receivedMsg}");
                    }
                }

                if (toSendMsgs.TryDequeue(out string toSendMsg))
                {
                    var parsed = toSendMsg.Split(' ');
                    if (parsed[0].ToUpper() == "QUIT")
                    {
                        Environment.Exit(-1);
                    }
                    else if (parsed[0].ToUpper() == "SEND" && parsed.Length == 4)
                    {
                        sender.Send(parsed[1], parsed[2], parsed[3]);
                    }
                    else
                    {
                        Console.WriteLine("Malformed input. Enter send <ip> <port> <message>, or quit to exit.");
                    }
                }
            }
        }

        static void EnqueueToSend(ConcurrentQueue<string> toSendMsgs)
        {
            while(true)
            {
                var toSendMsg = Console.ReadLine();
                toSendMsgs.Enqueue(toSendMsg);
            }
        }

        static Dictionary<string, string> GetArguments(string[] args)
        {
            var arguments = new Dictionary<string, string>();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Length > 1 && args[i][0] == '-')
                {
                    arguments.TryAdd(args[i][1..], args[i + 1]);
                    i++;
                }
            }
            return arguments;
        }
    }
} 