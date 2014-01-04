using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Couchbase.Tests
{
    [TestFixture]
    public class Onemore
    {
        private Byte[] data = new Byte[2048];
        private int size = 2048;
        private Socket server;
        static ManualResetEvent allDone = new ManualResetEvent(false);

        [Test]
        public void Test()
        {
            Run();
        }

        [TearDown]
        public void TearDown()
        {
            Thread.Sleep(5000);
        }

        public void Run()
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 33333);
                server.Bind(iep);
                Console.WriteLine("Server initialized..");
                server.Listen(100);
                Console.WriteLine("Listening...");
                while (true)
                {
                    allDone.Reset();
                    server.BeginAccept(new AsyncCallback(AcceptCon), server);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        void AcceptCon(IAsyncResult iar)
        {
            allDone.Set();
            try
            {
                Socket oldserver = (Socket)iar.AsyncState;
                Socket client = oldserver.EndAccept(iar);
                Console.WriteLine(client.RemoteEndPoint.ToString() + " connected");
                byte[] message = Encoding.ASCII.GetBytes("Welcome");
                client.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendData), client);
            }
            catch (Exception)
            {
                Console.WriteLine("Connection closed..");
                return;
            }
        }

        void SendData(IAsyncResult iar)
        {
            try
            {
                Socket client = (Socket)iar.AsyncState;
                int sent = client.EndSend(iar);
                client.BeginReceive(data, 0, size, SocketFlags.None, new AsyncCallback(ReceiveData), client);
            }
            catch (Exception)
            {
                Console.WriteLine("Connection closed..");
                return;
            }
        }

        void ReceiveData(IAsyncResult iar)
        {
            try
            {
                Socket client = (Socket)iar.AsyncState;
                int recv = client.EndReceive(iar);
                if (recv == 0)
                {
                    client.Close();
                    server.BeginAccept(new AsyncCallback(AcceptCon), server);
                    return;
                }
                string receivedData = Encoding.ASCII.GetString(data, 0, recv);
                // process received data here
                // decide what to send back
                byte[] message2 = Encoding.ASCII.GetBytes("reply");
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None, new AsyncCallback(SendData), client);
            }
            catch (Exception)
            {
                Console.WriteLine("Connection closed..");
                return;
            }
        }
    }
}
