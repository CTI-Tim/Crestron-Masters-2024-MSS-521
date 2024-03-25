using Crestron.SimplSharp.CrestronSockets;
using Crestron.SimplSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M24DriversDemo
{
    /// <summary>
    /// Just a basic class to act as a fake device to connect to so R data can be displayed on the UI
    /// </summary>
    public static class DriverEmulation
    {

        static TCPServer _server;
        static int _portNumber;

        public delegate void commandDelegate(string parms);

        public static void Initialize(int portNumber)
        {
            _portNumber = portNumber;
            _server = new TCPServer(IPAddress.Any.ToString(), _portNumber, 15000, EthernetAdapterType.EthernetUnknownAdapter, 10);
            _server.SocketSendOrReceiveTimeOutInMs = 100000;
            _server.WaitForConnectionAsync(ClientConnected);
        }

        static public void Disconnect()
        {
            if (_server != null)
            {
                _server.DisconnectAll();
                _server.Stop();
            }
        }

        static public void Send(string str)
        {
            byte[] b = str.ToString().ToCharArray().Select(c => (byte)c).ToArray();
            _server.SendDataAsync(b, b.Length, sentCallback);
        }


        static private void sentCallback(TCPServer server, uint clientindex, int size)
        {

        }

        static private void ClientConnected(TCPServer s, uint index)
        {
            _server.ReceiveDataAsync(ReceiveData);
        }

        static private void ReceiveData(TCPServer s, uint index, int bytes)
        {
            string IncommingRx = Encoding.Default.GetString(s.GetIncomingDataBufferForSpecificClient(index), 0, bytes);
            _server.ReceiveDataAsync(ReceiveData);
            Global.M24DemoUI.SetJoin(30, IncommingRx);
        }

    }
}