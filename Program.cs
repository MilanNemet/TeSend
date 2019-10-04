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
        static string port;
        static string partner = "Remote Host";

        static void Main(string[] args)
        {
            localIP = GetLocalIP();
            Console.WriteLine("Your IP: " + localIP);

            Console.WriteLine("Remote IP address:");
            string remoteIP = Console.ReadLine();
            //Console.WriteLine("Port number:");
            //string port = Console.ReadLine();
            

            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                local_EP = new IPEndPoint(IPAddress.Parse(localIP), 5000);
                remote_EP = new IPEndPoint(IPAddress.Parse(remoteIP), 5000);

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
            #region obsolete code path
            //try
            //{
            //    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
            //    client.Connect(endPoint);

            //    Thread Sender = new Thread(() => SendMessage(client));
            //    Thread Receiver = new Thread(() => ReceiveMessage(client));

            //    Receiver.Start();
            //    SendMessage(client);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            //finally
            //{
            //    client.Close();
            //    Console.WriteLine("End of transmission!");
            //}
            //Console.ReadKey();
            #endregion
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
                    //Console.WriteLine(input);
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
        static string GetLocalIP()
        {
            string localAddress;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localAddress = endPoint.Address.ToString();
            }
            return localAddress;
        }
    }
}
