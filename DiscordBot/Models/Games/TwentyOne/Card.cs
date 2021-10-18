using DiscordBot.Models.Games.TwentyOne.CardEnums;

namespace DiscordBot.Models.Games.TwentyOne
{
    public struct Card
    {
        public Suit Suit { get; private set; }
        public string Name { get; private set; }
        public int Cost { get; private set; }
        public Card(Suit suit,string name, int cost)
        {
            Suit = suit;
            Name = name;
            Cost = cost;
        }
        public Color Color => (Suit == Suit.Heart || Suit == Suit.Diamond) ? Color.Red : Color.Black;
        public string FullName()
        {
            string fullname = string.Empty;
            switch (Suit)
            {
                case Suit.Heart:
                    fullname += ":hearts:";
                    break;
                case Suit.Diamond:
                    fullname += ":diamonds:";
                    break;
                case Suit.Club:
                    fullname += ":clubs:";
                    break;
                case Suit.Spade:
                    fullname += ":hearts:";
                    break;
            }
            fullname += $" {Name}";
            return fullname;
        }
    }
}