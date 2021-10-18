using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot.Models.Games.TwentyOne
{
    public class Player
    {
        public List<Card> Cards { get; private set; }
        public IUser User { get; private set; }
        public int Score => GetPoints();
        public bool Stopped { get; private set; }
        public Player(IUser user)
        {
            User = user;
            Stopped = false;
            Cards = new List<Card>();
        }
        public void Stop() { Stopped = true; }

        private int GetPoints()
        {
            int points = 0;
            for(int i = 0; i < Cards.Count; i++)
            {
                if (i > 0 && Cards[i].Name == "Ace")
                    points++;
                else points += Cards[i].Cost;
            }
            return points;
        }
        public void AddCard(Card card)
        {
            Cards.Add(card);
            Task.Run(async () => await User.SendMessageAsync($"You get {card.FullName()}. Your score is {Score}"));
        }
        public void AddCards(List<Card> cards)
        {
            Cards.AddRange(cards);
            string mes = string.Empty;
            foreach(var card in cards)
            {
                mes += card.FullName() + "  ";
            }
            Task.Run(async () => await User.SendMessageAsync($"You get {mes}. Your score is {Score}"));
        }
    }
}
