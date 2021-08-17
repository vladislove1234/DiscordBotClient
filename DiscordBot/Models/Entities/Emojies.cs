using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models.Entities
{
    public static class Emojies
    {
        public static Emoji One => new Emoji("\x0031\xFE0F\x20E3");
        public static Emoji Two => new Emoji("\x0032\xFE0F\x20E3");
        public static Emoji Three => new Emoji("\x0033\xFE0F\x20E3");
        public static Emoji Four => new Emoji("\x0034\xFE0F\x20E3");
        public static Emoji Five => new Emoji("\x0035\xFE0F\x20E3");

        public static Emoji GetNumberFromInt(int i)
        {
            switch (i)
            {
                case 1:
                    return One;
                case 2:
                    return Two;
                case 3:
                    return Three;
                case 4:
                    return Four;
                case 5:
                    return Five;
                default:
                    return Three;
            }
            return Four;
        }

    }
}
