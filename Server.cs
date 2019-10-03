using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            socket server = null;
            socket client = null;

            try
            {
               server = new socket(addressfamily.internetwork, sockettype.stream, protocoltype.tcp);
               ipendpoint endpoint = new ipendpoint(ipaddress.parse("127.0.0.1"), 5000);

               server.bind(endpoint);
               server.listen(2);
               client = server.accept();

               thread sender = new thread(() => SendMessage(client));
               thread receiver = new thread(() => ReceiveMessage(client));

               receiver.start();
               sender.start();
            }
            catch (exception e)
            {
               console.writeline(e.message);
            }
            finally
            {
               client.close();
               server.close();
               console.writeline("End of transmission!");
            }
            console.readkey();

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
                    Console.WriteLine($"Remote Client: {Encoding.UTF8.GetString(data, 0, length)}");
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
