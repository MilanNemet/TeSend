using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TeSend
{
    class Program
    {
        static Socket server = null;
        static Socket client = null;
        static IPEndPoint local_EP = null;
        static IPEndPoint remote_EP = null;
        static string localIP;
        static string remoteIP;
        static int port = 52052;
        static string partner = "Remote Host";

        static void Main(string[] args)
        {
            local_EP = GetLocalIPEndPoint();
            localIP = local_EP.Address.ToString();
            local_EP.Port = port;
            Console.WriteLine("Your IP: " + localIP);
            Console.WriteLine("Port: " + local_EP.Port);

            Console.WriteLine("Remote IP address:");
            remoteIP = Console.ReadLine();
            remote_EP = new IPEndPoint(IPAddress.Parse(remoteIP), port);


            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Console.ForegroundColor = ConsoleColor.Yellow;

                try
                {
                    Console.WriteLine($"Connecting to \"{partner}\"...");
                    client.Connect(remote_EP);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Connection timed out!\n" +
                        $"Waiting for \"{partner}\" to join in...");
                    server.Bind(local_EP);
                    server.Listen(2);
                    client = server.Accept();
                }
                finally
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection established.");
                    Console.ResetColor();
                }

                Thread Receiver = new Thread(() => ReceiveMessage(client));
                Receiver.Start();
                SendMessage(client);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                client.Close();
                server.Close();
                Console.WriteLine("End of transmission!");
            }
            Console.ReadKey();
        }
        static void SendMessage(Socket _client)
        {
            string input = "init";
            while (input.Length != 0)
            {
                Socket client = _client as Socket;
                try
                {
                    input = Console.ReadLine();
                    byte[] data = new byte[256];
                    data = Encoding.UTF8.GetBytes(input);
                    client.Send(data, data.Length, SocketFlags.None);                                                                                                                                                                                                                                                                                               
                    Thread.Sleep(200);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
        }
        static void ReceiveMessage(Socket _client)
        {
            client = _client as Socket;
            while (true)
            {
                try
                {
                    byte[] data = new byte[256];
                    int length = client.Receive(data);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{partner}: {Encoding.UTF8.GetString(data, 0, length)}");
                    Console.ResetColor();
                    Thread.Sleep(500);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
        }
        static IPEndPoint GetLocalIPEndPoint()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", port);
                return socket.LocalEndPoint as IPEndPoint;
            }
        }
    }
}
