using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BiampTest
{
    public class TcpTestServer : IDisposable
    {

        private const int BUFFER_SIZE = 1048;

        public delegate void DataArrivedHandler(object sender, DataArrivedEventArgs e);
        public event DataArrivedHandler DataArrived;

        private TcpListener _listener = null;
        private Thread _worker = null;
        private string _ipAddress = "127.0.0.1";
        private int _port = -1;

        public TcpTestServer(int port) : this("127.0.0.1", port)
        {

        }

        public TcpTestServer(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public void Start()
        {
            Close();
            _listener = new TcpListener(IPAddress.Parse(_ipAddress), _port);

            _worker = new Thread(new ThreadStart(DoWork));
            _worker.Start();
        }

        public void Stop()
        {

        }

        public void Close()
        {
            Dispose();
        }

        protected void OnDataArrived(DataArrivedEventArgs e)
        {
            if (DataArrived != null)
                DataArrived(this, e);
        }

        public void DoWork()
        {

            _listener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    NetworkStream s = client.GetStream();
                    byte[] buffer = new byte[BUFFER_SIZE];
                    MemoryStream ms = new MemoryStream();

                    int totalCount = 0;
                    while (s.DataAvailable)
                    {
                        int count = s.Read(buffer, 0, buffer.Length);
                        totalCount += count;
                        if (count == 0)
                            break;
                        ms.Write(buffer, 0, count);
                    }

                    if (totalCount > 0)
                        OnDataArrived(new DataArrivedEventArgs(ms.ToArray()));

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (_listener != null)
                {
                    _listener.Stop();
                    _listener = null;
                }

            }
            catch (Exception ex)
            {
                // ignore 
            }

            try
            {
                if (_worker != null)
                {
                    _worker.Abort();
                    _worker = null;
                }

            }
            catch (Exception ex)
            {
                // ignore 
            }

        }

        #endregion
    }


    public class DataArrivedEventArgs : EventArgs
    {

        private byte[] _data = null;

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public DataArrivedEventArgs(byte[] data)
        {
            _data = data;
        }


    }
}
