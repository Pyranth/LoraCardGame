using Client;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoraCardGame
{
    public delegate void ConnectionToServerHandler();
    public delegate void MessageToGUIHandler(string message);
    public class Communication
    {
        private IServer server;
        private IClient client;

        public event ConnectionToServerHandler ConnectionFailed;
        public event MessageToGUIHandler MessageToGUIRecieved;

        public void StartServer()
        {
            if (server != null)
                return;

            server = new GameServer();
            server.Start();
        }

        public void StartClient()
        {
            if (client != null)
                return;

            client = new GameClient();
            client.MessageRecieved += OnMessageRecieved;

            if (client.ConnectToServer() == false)
            {
                ConnectionFailed();
            }

            client.StartListening();
        }

        public void StopServer()
        {
            if (server == null)
                return;

            server.Stop();
        }

        public void StopClient()
        {
            if (client == null)
                return;

            client.Stop();
        }

        public void SendMessage(string message)
        {
            client.SendMessage(message);
        }

        private void OnMessageRecieved(string message)
        {
            MessageToGUIRecieved(message);
        }
    }
}
