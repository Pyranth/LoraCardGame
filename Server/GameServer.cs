using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using GameEngine;

namespace Server
{
    public class GameServer : IServer
    {
        int numberOfPlayers;
        int numberOfConnectedPlayers;
        bool isServerFull;

        private Thread[] backgroundThreads;
        private TcpListener tcpListener;
        private TcpClient[] tcpClients;
        private NetworkStream[] networkStreams;

        private GameManager gameManager;

        public GameServer()
        {
            numberOfPlayers = 4;
            numberOfConnectedPlayers = 0;
            isServerFull = false;

            backgroundThreads = new Thread[numberOfPlayers + 1];
            tcpListener = new TcpListener(IPAddress.Any, 9050);
            tcpClients = new TcpClient[numberOfPlayers + 1];
            networkStreams = new NetworkStream[numberOfPlayers + 1];

            gameManager = new GameManager();
            gameManager.StartGame();
        }

        public bool Start()
        {
            try
            {
                tcpListener.Start();

                // For loop doesnt work !
                backgroundThreads[0] = new Thread(() => Listen(0));
                backgroundThreads[0].Start();

                backgroundThreads[1] = new Thread(() => Listen(1));
                backgroundThreads[1].Start();

                backgroundThreads[2] = new Thread(() => Listen(2));
                backgroundThreads[2].Start();

                backgroundThreads[3] = new Thread(() => Listen(3));
                backgroundThreads[3].Start();
            }
            catch (Exception)
            {
                tcpListener.Stop();

                foreach (Thread t in backgroundThreads)
                    t?.Abort();

                return false;
            }

            //backgroundThreads[4] = new Thread(() => Listen(4));
            //backgroundThreads[4].Start();

            return true;
        }
        public void Stop()
        {
            string message = "exit;\n\r";
            byte[] messageByte = Encoding.ASCII.GetBytes(message);

            foreach (NetworkStream ns in networkStreams)
                ns?.Write(messageByte, 0, messageByte.Length);

            foreach (Thread backgroundThread in backgroundThreads)
                backgroundThread?.Abort();

            foreach (NetworkStream networkStream in networkStreams)
                networkStream?.Close();

            foreach (TcpClient tcpClient in tcpClients)
                tcpClient?.Close();

            tcpListener.Stop();
        }

        private void Listen(int i)
        {
            tcpClients[i] = tcpListener.AcceptTcpClient();

            if (tcpClients[i].Connected)
            {
                tcpClients[i].NoDelay = true;
                networkStreams[i] = tcpClients[i].GetStream();

                SendMessageRegisterPlayer(i, gameManager.RegisterPlayer());

                if (PlayerConnected() == false)
                {
                    byte[] message = Encoding.ASCII.GetBytes("server_full");
                    networkStreams[i].Write(message, 0, message.Length);
                    networkStreams[i].Close();
                    tcpClients[i].Close();

                    backgroundThreads[i].Abort();
                    backgroundThreads[i].Start();
                }

                while (true)
                {
                    if (tcpClients[i].Connected == false)
                        break;

                    byte[] messageReadByte = new byte[tcpClients[i].ReceiveBufferSize];
                    int bytesRead = networkStreams[i].Read(messageReadByte, 0, tcpClients[i].ReceiveBufferSize);
                    string messageRead = Encoding.ASCII.GetString(messageReadByte, 0, bytesRead);

                    ParseMessage(messageRead);

                    string[] data = messageRead.Split(';');

                    if (data[0] == "player_exit")
                    {
                        tcpClients[i].Client.Disconnect(true);
                        break;
                    }
                    /*
                    foreach (NetworkStream networkStream in networkStreams)
                        networkStream?.Write(messageReadByte, 0, bytesRead);
                    */

                }

                networkStreams[i].Close();
                networkStreams[i].Dispose();
                networkStreams[i] = null;
            }

            tcpClients[i].Close();
            tcpClients[i] = null;

            SendMessageResetGame();

            ResetThread(i);
        }

        private void ResetThread(int i)
        {
            backgroundThreads[i] = new Thread(() => Listen(i));
            backgroundThreads[i].Start();
        }

        private bool PlayerConnected()
        {
            if (isServerFull == false)
            {
                numberOfConnectedPlayers++;

                if (numberOfConnectedPlayers == 4)
                {
                    isServerFull = true;
                    //SendMessageStartGame();
                }

                return true;
            }
            return false;
        }

        private void SendMessageStartGame()
        {
            if (isServerFull == false)
                return;

            SendPlayerCards();
            Thread.Sleep(100);
            SendPlayersInfo();
            Thread.Sleep(100);
            SendForbidPlay();
            Thread.Sleep(100);
            SendAllowPlay();
            Thread.Sleep(100);
        }

        private void SendMessageRegisterPlayer(int i, int number)
        {
            string message = "register;" + number.ToString() + ";\n\r";
            byte[] messageByte = Encoding.ASCII.GetBytes(message);

            SendMessage(messageByte, networkStreams[i]);
        }

        private void SendErrorMessage(string error)
        {
            string message = "error;";
            message += error;
            byte[] messageByte = Encoding.ASCII.GetBytes(message);

            foreach (NetworkStream ns in networkStreams)
                SendMessage(messageByte, ns);
        }

        private void ParseMessage(string message)
        {
            string[] data = message.Split(';');

            switch (data[0])
            {
                // data[1] player number
                // data[2] value
                // data[3] suit
                case "move":
                    SendForbidPlay();

                    if (gameManager.IsMoveValid(Convert.ToInt32(data[1]), data[2], data[3]) == false)
                    {
                        string msg = "invalid_move;";
                        foreach (NetworkStream ns in networkStreams)
                            SendMessage(Encoding.ASCII.GetBytes(msg), ns);

                        SendAllowPlay();

                        break;
                    }

                    gameManager.PlayerMove(Convert.ToInt32(data[1]), data[2], data[3]);

                    foreach (NetworkStream networkStream in networkStreams)
                        SendMessage(Encoding.ASCII.GetBytes(message), networkStream);

                    CheckIfTurnEnd();
                    CheckIfRoundEnd();
                    SendAllowPlay();
                    break;

                case "start_game":
                    if (isServerFull)
                    {
                        SendMessageStartGame();
                    }
                    else
                    {
                        SendErrorMessage("Not enough players are connected to start game!");
                    }
                    break;

                case "player_exit":
                    numberOfConnectedPlayers--;
                    isServerFull = false;
                    gameManager.ResetGame();

                    //SendMessageResetGame();

                    break;

                default:
                    break;
            }
        }

        private void SendMessage(byte[] messageByte, NetworkStream networkStream)
        {
            if (networkStream == null || networkStream.CanWrite == false)
                return;
            networkStream?.Write(messageByte, 0, messageByte.Length);
            Thread.Sleep(20);
        }

        private void SendMessageResetGame()
        {
            foreach(NetworkStream ns in networkStreams)
            {
                SendMessage(Encoding.ASCII.GetBytes("reset;"), ns);
            }
            SendForbidPlay();
        }

        private void SendPlayerCards()
        {
            string message;
            byte[] messageByte;

            for (int i = 0; i < 4; i++)
            {
                message = "deal_cards;" + i.ToString() + ";" + gameManager.GetPlayersHand(i) + ";\n\r";
                messageByte = Encoding.ASCII.GetBytes(message);

                foreach (NetworkStream ns in networkStreams)
                    SendMessage(messageByte, ns);
            }

        }

        private void SendPlayersInfo()
        {
            string message;
            byte[] messageByte;

            for (int i = 1; i <= 4; i++)
            {
                message = string.Empty;
                message = "get_players;" + (i - 1).ToString() + ";";
                message += ((i + 1) % 4).ToString() + ";";
                message += ((i + 2) % 4).ToString() + ";";
                message += ((i + 3) % 4).ToString() + ";\n\r";

                messageByte = Encoding.ASCII.GetBytes(message);

                foreach (NetworkStream ns in networkStreams)
                    SendMessage(messageByte, ns);
            }
        }

        private void SendForbidPlay()
        {
            string message;
            byte[] messageByte;

            message = "forbid_play;\n\r";
            messageByte = Encoding.ASCII.GetBytes(message);

            foreach (NetworkStream ns in networkStreams)
                SendMessage(messageByte, ns);
        }

        private void SendAllowPlay()
        {
            string message;
            byte[] messageByte;

            message = "allow_play;" + gameManager.GetPlayersTurn().ToString() + ";\n\r";
            messageByte = Encoding.ASCII.GetBytes(message);

            foreach (NetworkStream ns in networkStreams)
                SendMessage(messageByte, ns);
        }

        private void SendTurnEnd()
        {
            string message;
            byte[] messageByte;

            message = "turn_end;\n\r";
            messageByte = Encoding.ASCII.GetBytes(message);

            foreach (NetworkStream ns in networkStreams)
                SendMessage(messageByte, ns);
        }

        private void SendPlayerPoints()
        {
            string message;
            byte[] messageByte;

            int[] playerPoints = gameManager.GetPlayerPoints();

            for (int i = 0; i < 4; i++)
            {
                message = "player_points;";
                message += i.ToString() + ";" + playerPoints[i].ToString() + ";\n\r";
                messageByte = Encoding.ASCII.GetBytes(message);

                foreach (NetworkStream ns in networkStreams)
                    SendMessage(messageByte, ns);
            }
        }

        private void CheckIfTurnEnd()
        {
            if (gameManager.TurnEnd == true)
            {
                SendPlayerPoints();
                SendTurnEnd();
                gameManager.TurnEnd = false;
            }
        }

        private void CheckIfRoundEnd()
        {
            if (gameManager.GameEnd == true)
            {
                string message = "game_end;";
                message += gameManager.GetWinner().ToString() + ";";
                byte[] messageByte = Encoding.ASCII.GetBytes(message);

                foreach (NetworkStream ns in networkStreams)
                    SendMessage(messageByte, ns);

                return;
            }

            if (gameManager.RoundEnd == true)
            {
                SendPlayerCards();
                gameManager.RoundEnd = false;
            }
        }
    }
}
