using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models.Games.Tanks
{
    public class Bullet
    {
        public Position Position;
        public VectorOfMove Vector;
        public Player Parent;
        public Bullet(Position pos, VectorOfMove vec, Player parent)
        {
            Position = pos;
            Vector = vec;
            Parent = parent;
        }

        public void Move()
        {
            switch (Vector)
            {
                case VectorOfMove.Up:
                    Position.X -= 1;
                    break;
                case VectorOfMove.Down:
                    Position.X += 1;
                    break;
                case VectorOfMove.Left:
                    Position.Y -= 1;
                    break;
                case VectorOfMove.Right:
                    Position.Y += 1;
                    break;
            }
        }
    }
}
