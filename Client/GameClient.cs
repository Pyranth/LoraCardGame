using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Client
{
    public delegate void MessageRecievedHandler(string message);

    public class GameClient : IClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private Thread listenToMessages;

        public event MessageRecievedHandler MessageRecieved;

        public GameClient()
        {
            listenToMessages = new Thread(new ThreadStart(ListenToServer));
            listenToMessages.SetApartmentState(ApartmentState.STA);
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public bool ConnectToServer()
        {
            bool connectionSucceded = false;

            List<IPAddress> ipAddressList = new List<IPAddress>();

            //Generating 192.168.0.1/24 IP Range
            for (int i = 8; i < 255; i++)
            {
                ipAddressList.Add(IPAddress.Parse("192.168.1." + i));
            }

            foreach (IPAddress ip in ipAddressList)
            {
                byte[] macAddr = new byte[6];
                uint macAddrLen = (uint)macAddr.Length;
                int uintAddress = BitConverter.ToInt32(ip.GetAddressBytes(), 0);

                if (SendARP(uintAddress, 0, macAddr, ref macAddrLen) == 0)
                {
                    try
                    {
                        tcpClient = new TcpClient();
                        var result = tcpClient.BeginConnect(ip.ToString(), 9050, null, null);
                        var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(100));

                        if (!success)
                            throw new SocketException();

                        tcpClient.EndConnect(result);
                        tcpClient.NoDelay = true;
                        connectionSucceded = true;
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                        {
                            tcpClient.Close();
                        }
                    }
                }

                if (connectionSucceded == true)
                    break;
            }

            if (connectionSucceded)
                networkStream = tcpClient.GetStream();

            return connectionSucceded;
        }

        public void StartListening()
        {
            listenToMessages.Start();
        }

        public void Stop()
        {
            SendMessage("player_exit;");

            listenToMessages?.Abort();

            networkStream?.Close();
            tcpClient?.Close();
        }

        public void SendMessage(string message)
        {
            byte[] messageSendByte = Encoding.ASCII.GetBytes(message);
            networkStream.Write(messageSendByte, 0, messageSendByte.Length);
        }

        private void ListenToServer()
        {
            while (true)
            {
                byte[] messageReadByte = new byte[tcpClient.ReceiveBufferSize];
                int bytesRead = networkStream.Read(messageReadByte, 0, tcpClient.ReceiveBufferSize);
                ParseMessage(messageReadByte);
            }
        }

        private void ParseMessage(byte[] messageReadByte)
        {
            MessageRecieved(Encoding.ASCII.GetString(messageReadByte));
        }
    }
}
