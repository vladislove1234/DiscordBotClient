using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models.Games.Tanks.Fields
{
    public class Field
    {
        public string Element { get; private set; }
        public bool CanEnter { get; private set; }
        public bool CanShoot { get; private set; }
        public Field(string element, bool canEnter, bool canShoot)
        {
            Element = element;
            CanEnter = canEnter;
            CanShoot = canShoot;
        }
        public static Field Wall = new Field(":bricks:", false,false);
        public static Field Empty = new Field(":black_large_square:", true, true);
        public static Field Bushes = new Field(":deciduous_tree:", true, true);
        public static Field Water = new Field(":deciduous_tree:", true, true);
    }
}
