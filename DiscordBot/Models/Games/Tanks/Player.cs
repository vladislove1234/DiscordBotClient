using System;
using Discord;

namespace DiscordBot.Models.Games.Tanks
{
    public class Player
    {
        public IUser User { get; private set; }
        public Position Position;
        public bool HasMove { get; set; }
        public VectorOfMove vector { get; set; }
        public int Score { get; private set; }
        public int ShootPeriod;
        public bool IsDead { get; private set; }
        public string Emoji { get; internal set; }

        public Player(IUser user)
        {
            this.Emoji = $":regional_indicator_{user.Username.ToCharArray()[0].ToString().ToLower()}:"; 
            ShootPeriod = 3;
            HasMove = true;
            User = user;
            vector = VectorOfMove.Up;
        }
        public void AddScore(int amount) {
            if (amount > 0)
                Score += amount;
            else throw new Exception("Wrong amount of added score!");
        }
        public void Kill()
        {
            IsDead = true;
        }
    }
}
