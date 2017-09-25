using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameEngine.Models;

namespace GameEngine
{
    public class GameManager
    {
        List<CardModel> deck;
        List<List<CardModel>> playerHands;
        int[] playerPoints;
        bool[] playerRegistered;
        int playersTurn;
        int playersTurnFirst;
        string firstSuit;
        int round;
        int roundMoves;
        bool turnEnd;
        bool roundEnd;
        bool gameEnd;

        Dictionary<int, CardModel> cardsOnTable;
        public GameManager()
        {
            Deck = new List<CardModel>();
            PlayerHands = new List<List<CardModel>>();
            for (int i = 0; i < 4; i++)
                PlayerHands.Add(new List<CardModel>());
            playerPoints = new int[4];
            playerRegistered = new bool[4];

            CardsOnTable = new Dictionary<int, CardModel>();

            TurnEnd = false;
            RoundEnd = false;
            GameEnd = false;
        }

        internal List<CardModel> Deck { get => deck; set => deck = value; }
        internal List<List<CardModel>> PlayerHands { get => playerHands; set => playerHands = value; }
        public Dictionary<int, CardModel> CardsOnTable { get => cardsOnTable; set => cardsOnTable = value; }
        public bool TurnEnd { get => turnEnd; set => turnEnd = value; }
        public bool RoundEnd { get => roundEnd; set => roundEnd = value; }
        public bool GameEnd { get => gameEnd; set => gameEnd = value; }

        public void StartGame()
        {
            Array.Clear(playerPoints, 0, 4);
            Array.Clear(playerRegistered, 0, playerRegistered.Length);
            playersTurn = 0;
            playersTurnFirst = 0;
            round = 1;
            roundMoves = 0;

            FillDeck();
            ShuffleDeck();
            DealCards();
        }

        public void ResetGame()
        {
            Array.Clear(playerPoints, 0, 4);
            Array.Clear(playerRegistered, 0, playerRegistered.Length);
            playersTurn = 0;
            playersTurnFirst = 0;
            round = 1;
            roundMoves = 0;

            foreach (List<CardModel> list in playerHands)
                list.Clear();
            CardsOnTable.Clear();

            FillDeck();
            ShuffleDeck();
            DealCards();
        }

        public string GetPlayersHand(int i)
        {
            string result = "";

            foreach (CardModel card in PlayerHands[i])
                result += card.Value + ";" + card.Suit + ";";

            return result;
        }

        public int RegisterPlayer()
        {
            for (int i = 0; i < 4; i++)
            {
                if (playerRegistered[i] == false)
                {
                    playerRegistered[i] = true;
                    return i;
                }
            }
            return -1;
        }

        public int GetPlayersTurn()
        {
            return playersTurn;
        }

        public bool IsMoveValid(int player, string value, string suit)
        {
            if (playersTurnFirst == player)
                return true;

            if (suit != firstSuit)
            {
                if (PlayerHands[player].Any(m => m.Suit == firstSuit))
                    return false;
                return true;
            }

            return true;
        }

        public void PlayerMove(int player, string value, string suit)
        {
            if (CardsOnTable.Count == 0)
                firstSuit = suit;

            PlayerHands[player].RemoveAll(m => m.Value == value && m.Suit == suit);
            CardsOnTable.Add(playersTurn, new CardModel(value, suit));
            playersTurn = (playersTurn + 1) % 4;
            roundMoves++;

            if (CardsOnTable.Count == 4)
            {
                var CardsList = CardsOnTable.Values.ToList();
                var CardsIndexList = CardsOnTable.Keys.ToList();

                int max = 0;

                for (int i = 1; i < 4; i++)
                {
                    if (CardsList[i] >= CardsList[max] && CardsList[i].Suit == firstSuit)
                        max = i;
                }

                playerPoints[CardsIndexList[max]]++;
                TurnEnd = true;

                CardsOnTable.Clear();
            }

            if (roundMoves == 32)
            {
                roundMoves = 0;
                round++;

                if (round == 5)
                    GameEnd = true;

                playersTurn = (playersTurn + 1) % 4;
                playersTurnFirst = (playersTurnFirst + 1) % 4;

                FillDeck();
                ShuffleDeck();
                DealCards();

                RoundEnd = true;
            }
        }

        public int[] GetPlayerPoints()
        {
            return playerPoints;
        }

        public int GetWinner()
        {
            int result = 0;

            for (int i = 1; i < 4; i++)
            {
                if (playerPoints[i] > playerPoints[result])
                    result = i;
            }

            return result;
        }

        private void DealCards()
        {
            for (int i = 0; i < Deck.Count; i++)
                PlayerHands[i % 4].Add(Deck[i]);
        }

        private void FillDeck()
        {
            string[] values = { "7", "8", "9", "10", "jack", "queen", "king", "ace" };
            string[] suits = { "hearts", "clubs", "spades", "diamonds" };

            Deck.Clear();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Deck.Add(new CardModel(values[j], suits[i]));
                }
            }
        }
        private void ShuffleDeck()
        {
            Deck.Sort();
            Random rand = new Random();
            for (int i = 0; i < Deck.Count - 1; i++)
            {
                int tmp = i + rand.Next(Deck.Count - i);

                CardModel card = Deck[i];
                Deck[i] = Deck[tmp];
                Deck[tmp] = card;
            }
        }
    }
}
