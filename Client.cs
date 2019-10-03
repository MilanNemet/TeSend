using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            Socket client = null;

            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
                client.Connect(endPoint);

                Thread Sender = new Thread(() => SendMessage(client));
                Thread Receiver = new Thread(() => ReceiveMessage(client));

                Receiver.Start();
                Sender.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                client.Close();
                Console.WriteLine("End of transmission!");
            }
            Console.ReadKey();
        }
		static void SendMessage(Socket client)
        {
            string input = "init";
            while (input.Length!=0)
            {
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
        static void ReceiveMessage(Socket client)
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[256];
                    int length = client.Receive(data);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Remote client: {Encoding.UTF8.GetString(data, 0, length)}");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
        }
    }
}
