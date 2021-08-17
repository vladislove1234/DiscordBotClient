using System;
using Discord;

namespace DiscordBot.Models.Games.Tanks
{
    public class Player
    {
        public IUser User { get; private set; }
        public Position Position;
        public bool HasMove = true;
        public Player(IUser user)
        {
            User = user;
        }
    }
}
