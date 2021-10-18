using System;
using System.Collections.Generic;

namespace DiscordBot.Models.Games.TwentyOne
{
    public class Deck
    {
        private List<Card> _36CardsDeck = new List<Card>()
        {
            new Card(CardEnums.Suit.Heart,"2",2),
            new Card(CardEnums.Suit.Club,"2",2),
            new Card(CardEnums.Suit.Diamond,"2",2),
            new Card(CardEnums.Suit.Spade,"2",2),
            new Card(CardEnums.Suit.Heart,"3",3),
            new Card(CardEnums.Suit.Club,"3",3),
            new Card(CardEnums.Suit.Diamond,"3",3),
            new Card(CardEnums.Suit.Spade,"3",3),
            new Card(CardEnums.Suit.Heart,"4",4),
            new Card(CardEnums.Suit.Club,"4",4),
            new Card(CardEnums.Suit.Diamond,"4",4),
            new Card(CardEnums.Suit.Spade,"4",4),
            new Card(CardEnums.Suit.Heart,"5",5),
            new Card(CardEnums.Suit.Club,"5",5),
            new Card(CardEnums.Suit.Diamond,"5",5),
            new Card(CardEnums.Suit.Spade,"5",5),
            new Card(CardEnums.Suit.Heart,"6",6),
            new Card(CardEnums.Suit.Club,"6",6),
            new Card(CardEnums.Suit.Diamond,"6",6),
            new Card(CardEnums.Suit.Spade,"6",6),
            new Card(CardEnums.Suit.Heart,"7",7),
            new Card(CardEnums.Suit.Club,"7",7),
            new Card(CardEnums.Suit.Diamond,"7",7),
            new Card(CardEnums.Suit.Spade,"7",7),
            new Card(CardEnums.Suit.Heart,"8",8),
            new Card(CardEnums.Suit.Club,"8",8),
            new Card(CardEnums.Suit.Diamond,"8",8),
            new Card(CardEnums.Suit.Spade,"8",8),
            new Card(CardEnums.Suit.Heart,"9",9),
            new Card(CardEnums.Suit.Club,"9",9),
            new Card(CardEnums.Suit.Diamond,"9",9),
            new Card(CardEnums.Suit.Spade,"9",9),
            new Card(CardEnums.Suit.Heart,"10",10),
            new Card(CardEnums.Suit.Club,"10",10),
            new Card(CardEnums.Suit.Diamond,"10",10),
            new Card(CardEnums.Suit.Spade,"10",10),
            new Card(CardEnums.Suit.Heart,"Jack",10),
            new Card(CardEnums.Suit.Club,"Jack",10),
            new Card(CardEnums.Suit.Diamond,"Jack",10),
            new Card(CardEnums.Suit.Spade,"Jack",10),
            new Card(CardEnums.Suit.Heart,"Queen",10),
            new Card(CardEnums.Suit.Club,"Queen",10),
            new Card(CardEnums.Suit.Diamond,"Queen",10),
            new Card(CardEnums.Suit.Spade,"Queen",10),
            new Card(CardEnums.Suit.Heart,"King",10),
            new Card(CardEnums.Suit.Club,"King",10),
            new Card(CardEnums.Suit.Diamond,"King",10),
            new Card(CardEnums.Suit.Spade,"King",10),
            new Card(CardEnums.Suit.Heart,"Ace",11),
            new Card(CardEnums.Suit.Club,"Ace",11),
            new Card(CardEnums.Suit.Diamond,"Ace",11),
            new Card(CardEnums.Suit.Spade,"Ace",11)
        };
        public List<Card> Cards { get; private set; }
        public Deck()
        {
            Cards = new List<Card>(_36CardsDeck);
            Shuffle();
        }
        private void Shuffle()
        {
            var rand = new Random();
            Card tempCard;
            int tempNumber;
            for(int i = 0; i < Cards.Count; i++)
            {
                tempNumber = rand.Next(0, Cards.Capacity);
                tempCard = Cards[tempNumber];
                Cards[tempNumber] = Cards[i];
                Cards[i] = tempCard;
            }
        }
        public Card TakeFirst()
        {
            if (Cards.Count < 1)
                throw new Exception("deck has not cards");
            Card ret = Cards[0];
            Cards.RemoveAt(0);
            return ret;
        }
        public List<Card> TakeCards(int count)
        {
            List<Card> returnCards = new List<Card>();
            if (Cards.Count < count)
                throw new Exception("deck has not cards");
            for(int i = 0; i < count; i++)
            {
                Card temp = Cards[0];
                Cards.RemoveAt(0);
                returnCards.Add(temp);
            }
            return returnCards;
        }
    }
}
