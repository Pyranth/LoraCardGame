using LoraCardGame.Models;
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

namespace LoraCardGame.User_Controls
{
    public delegate void CardClickEvent(string message);

    /// <summary>
    /// Interaction logic for Card.xaml
    /// </summary>
    public partial class Card : UserControl
    {
        private bool played;
        private CardModel cardModel;
        public event CardClickEvent CardClick;

        public CardModel CardModel { get => cardModel; set => cardModel = value; }

        public Card(CardModel cm)
        {
            InitializeComponent();

            played = false;
            CardModel = cm;
            image.Source = new BitmapImage(new Uri(@"pack://application:,,,/LoraCardGame;component/Cards/" + CardModel.Value + "_of_" + CardModel.Suit + ".png", UriKind.Absolute));
        }

        public void CardPlayed()
        {
            played = true;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!played)
                CardClick(CardModel.Value + ";" + CardModel.Suit);
        }
    }
}
