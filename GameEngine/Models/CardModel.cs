using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Models
{
    public class CardModel : IComparable<CardModel>
    {
        private string value;
        private string suit;
        private int priority;

        public string Value { get => value; set => this.value = value; }
        public string Suit { get => suit; set => suit = value; }
        public int Priority { get => priority; set => priority = value; }

        public CardModel(string value, string suit)
        {
            Value = value;
            Suit = suit;
            SetPriority();
        }

        public override int GetHashCode()
        {
            return (Value + Suit).GetHashCode();
        }
        public int CompareTo(CardModel other)
        {
            return other.Priority.CompareTo(this.Priority);
        }
        public static bool operator >=(CardModel cm1, CardModel cm2)
        {
            return cm1.Priority >= cm2.Priority;
        }
        public static bool operator <=(CardModel cm1, CardModel cm2)
        {
            return cm1.Priority <= cm2.Priority;
        }
        private void SetPriority()
        {
            switch (Value)
            {
                case "1":
                    Priority = 1;
                    break;
                case "2":
                    Priority = 2;
                    break;
                case "3":
                    Priority = 3;
                    break;
                case "4":
                    Priority = 4;
                    break;
                case "5":
                    Priority = 5;
                    break;
                case "6":
                    Priority = 6;
                    break;
                case "7":
                    Priority = 7;
                    break;
                case "8":
                    Priority = 8;
                    break;
                case "9":
                    Priority = 9;
                    break;
                case "10":
                    Priority = 10;
                    break;
                case "jack":
                    Priority = 11;
                    break;
                case "queen":
                    Priority = 12;
                    break;
                case "king":
                    Priority = 13;
                    break;
                case "ace":
                    Priority = 14;
                    break;
            }
        }

        
    }
}
