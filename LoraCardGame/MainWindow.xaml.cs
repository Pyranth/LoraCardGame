using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Client;
using Server;

using LoraCardGame.Models;
using LoraCardGame.User_Controls;
using System.Collections.ObjectModel;

namespace LoraCardGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Communication communicationManager;

        private ObservableCollection<Card> cards = new ObservableCollection<Card>();
        private ObservableCollection<Card> cardsOnTable = new ObservableCollection<Card>();

        private bool canPlay;
        private int playerNumber;
        private int points;

        public ObservableCollection<Card> Cards { get => cards; set => cards = value; }
        public ObservableCollection<Card> CardsOnTable { get => cardsOnTable; set => cardsOnTable = value; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Initialize();

            communicationManager = new Communication();
            communicationManager.ConnectionFailed += OnConnectionFailed;
            communicationManager.MessageToGUIRecieved += OnMessageRecieved;
        }

        private void Initialize()
        {
            Player1.textBlockPlayer.Text = "Player";
            Player2.textBlockPlayer.Text = "Player";
            Player3.textBlockPlayer.Text = "Player";

            canPlay = false;
            playerNumber = 0;
            points = 0;
        }

        private void ParseMessage(string message)
        {
            string[] data = message.Split(';');

            switch (data[0])
            {
                case "register":
                    playerNumber = Convert.ToInt32(data[1]);
                    textBlock.Text = "Connected succesfully! Waiting for other players...";
                    buttonStart.Visibility = Visibility.Visible;
                    break;

                case "deal_cards":
                    textBlock.Text = "";
                    buttonStart.Visibility = Visibility.Collapsed;
                    if (Convert.ToInt32(data[1]) != playerNumber)
                        break;
                    for (int i = 2; i <= 17; i += 2)
                    {
                        CardModel model = new CardModel(data[i], data[i + 1]);
                        Card card = new Card(model);
                        card.CardClick += OnCardClick;

                        Cards.Add(card);
                    }

                    break;

                case "get_players":
                    if (Convert.ToInt32(data[1]) == playerNumber)
                    {
                        Player2.textBlockPlayer.Text = "Player " + (data[2] == "0" ? "4" : data[2].ToString());
                        Player3.textBlockPlayer.Text = "Player " + (data[3] == "0" ? "4" : data[3].ToString());
                        Player1.textBlockPlayer.Text = "Player " + (data[4] == "0" ? "4" : data[4].ToString());
                    }
                    break;

                case "allow_play":
                    if (Convert.ToInt32(data[1]) == playerNumber)
                        canPlay = true;
                    break;

                case "forbid_play":
                    canPlay = false;
                    break;

                case "move":
                    {
                        CardModel model = new CardModel(data[2], data[3]);
                        Card card = new Card(model);
                        card.CardPlayed();
                        CardsOnTable.Add(card);

                        if (Convert.ToInt32(data[1]) == playerNumber)
                            Cards.Remove(Cards.Single(m => m.CardModel.Value == data[2] && m.CardModel.Suit == data[3]));
                    }

                    break;

                case "player_points":
                    if (Convert.ToInt32(data[1]) == playerNumber)
                    {
                        points = (Convert.ToInt32(data[2]));
                    }
                    else
                    {
                        switch (data[1])
                        {
                            case "0":
                                if (Player1.textBlockPlayer.Text == "Player 1")
                                    Player1.textBlockPoints.Text = data[2];
                                if (Player2.textBlockPlayer.Text == "Player 1")
                                    Player2.textBlockPoints.Text = data[2];
                                if (Player3.textBlockPlayer.Text == "Player 1")
                                    Player3.textBlockPoints.Text = data[2];
                                break;
                            case "1":
                                if (Player1.textBlockPlayer.Text == "Player 2")
                                    Player1.textBlockPoints.Text = data[2];
                                if (Player2.textBlockPlayer.Text == "Player 2")
                                    Player2.textBlockPoints.Text = data[2];
                                if (Player3.textBlockPlayer.Text == "Player 2")
                                    Player3.textBlockPoints.Text = data[2];
                                break;
                            case "2":
                                if (Player1.textBlockPlayer.Text == "Player 3")
                                    Player1.textBlockPoints.Text = data[2];
                                if (Player2.textBlockPlayer.Text == "Player 3")
                                    Player2.textBlockPoints.Text = data[2];
                                if (Player2.textBlockPlayer.Text == "Player 3")
                                    Player3.textBlockPoints.Text = data[2];
                                break;
                            case "3":
                                if (Player1.textBlockPlayer.Text == "Player 4")
                                    Player1.textBlockPoints.Text = data[2];
                                if (Player2.textBlockPlayer.Text == "Player 4")
                                    Player2.textBlockPoints.Text = data[2];
                                if (Player3.textBlockPlayer.Text == "Player 4")
                                    Player3.textBlockPoints.Text = data[2];
                                break;
                            default:
                                break;
                        }
                    }
                    break;

                case "turn_end":
                    CardsOnTable.Clear();
                    break;

                case "game_end:":
                    MessageBox.Show("Winner is Player " + (Convert.ToInt32(data[1]) + 1).ToString());
                    break;

                case "reset":
                    Cards.Clear();
                    CardsOnTable.Clear();

                    buttonStart.Visibility = Visibility.Visible;

                    break;

                case "error":
                    MessageBox.Show(data[1]);
                    break;

                case "exit":
                    Close();
                    break;

                default:
                    break;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "Creating server...";

            communicationManager.StartServer();
            Join_Click(sender, e);
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "Connecting to server...";
            communicationManager.StartClient();
        }

        private void OnMessageRecieved(string message)
        {
           this.Dispatcher.BeginInvoke(new Action(() =>
           {
               ParseMessage(message);
           })); 
        }

        private void OnConnectionFailed()
        {
            MessageBox.Show("Connection failed!");
        }

        private void OnCardClick(string data)
        {
            if (canPlay == false)
                return;

            string message = "move;";
            message += playerNumber.ToString() + ";";
            message += data + ";";

            communicationManager.SendMessage(message);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            communicationManager.StopServer();
            communicationManager.StopClient();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            string message = "start_game;";

            communicationManager.SendMessage(message);
        }
    }
}
