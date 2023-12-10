using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BiampTest
{
    internal class Program
    {
        public static TcpTestServer ampdevice;
        private static int volume = 0;
        private static Int32 port = 6000;
        private static IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        //public delegate void DataArrivedHandler(object sender, DataArrivedEventArgs e);
        //public event DataArrivedHandler DataArrived;
        static void Main(string[] args)
        {
            bool EXIT = false;

            TcpTestServer ampdevice = new TcpTestServer(port);
            ampdevice.Start();
            ampdevice.DataArrived += new TcpTestServer.DataArrivedHandler(ampdevice_DataArrived);

            WriteTOConsole();
            while (!EXIT)
            {
                ConsoleKeyInfo dd = Console.ReadKey();
                switch(dd.Key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            Send("increment");
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            Send("decrement");
                            break;
                        }
                    case ConsoleKey.E:
                        {
                            EXIT = true;
                            break;
                        }
                }

            }

            ampdevice.Stop();
            Environment.Exit(0);
            
        }

        static void ampdevice_DataArrived(object sender, DataArrivedEventArgs e)
        {
            var recData = e.Data;
            string dataReceived = Encoding.ASCII.GetString(recData, 0, recData.Length);
            if (dataReceived.Contains("increment"))
            {
                SetTheVolume(1);
            }
            else if (dataReceived.Contains("decrement")){
                SetTheVolume(-1);
            }

            WriteTOConsole();
        }

        private static void Send(string inCvolume)
        {
            string message = $"Level1 {inCvolume} level 1 1" + Environment.NewLine;

            TcpClient client = new TcpClient();
            client.Connect(localAddr.ToString(), port);

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            NetworkStream nwStream = client.GetStream();
            nwStream.Write(data, 0, data.Length);

            byte[] bytesToRead = new byte[data.Length];
        }

        private static void SetTheVolume(int newvolume) {
            if (volume + newvolume > 10) {
                volume = 10;
            }
            else if (volume + newvolume < 0)
            {
                volume = 0;
            }
            else
            {
                volume += newvolume;
            }

        }
        private static void WriteTOConsole()
        {
            Console.Clear();
            Console.WriteLine("Press up or down keys to increase or decrease volume respectively. Press E to stop program.");
            Console.WriteLine(@$"Current Volume:{volume}");
        }

    }


}
