using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoraCardGame.Models
{
    public class CardModel
    {
        private string value;
        private string suit;

        public string Value { get => value; set => this.value = value; }
        public string Suit { get => suit; set => suit = value; }

        public CardModel(string value, string suit)
        {
            Value = value;
            Suit = suit;
        }
    }
}
