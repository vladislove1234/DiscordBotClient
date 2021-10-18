using System;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.Models.Games.Tanks.Fields;

namespace DiscordBot.Models.Games.Tanks
{
    public class GameField
    {        
        private string[,] stringField;
        public Field[,] CurrentField;
        public int X { get; private set; }
        public int Y { get; private set; }
        public List<Player> Players;
        public GameField(string[,] field, int sizeX, int sizeY)
        {
            Players = new List<Player>();
            X = sizeX;
            Y = sizeY;
            CurrentField = new Field[sizeX, sizeY];
            for(int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    switch (field[x, y].ToLower())
                    {
                        case "x":
                            CurrentField[x, y] = Field.Wall;
                            break;
                        case ".":
                            CurrentField[x, y] = Field.Empty;
                            break;
                        case "w":
                            CurrentField[x, y] = Field.Water;
                            break;
                        case "b":
                            CurrentField[x, y] = Field.Bushes;
                            break;
                        default:
                            CurrentField[x, y] = Field.Empty;
                            break;
                    }
                }
            }
        }
        public bool IsMovable(Position pos)
        {
            var p = Players.Where(x => x.Position.X == pos.X && x.Position.Y == pos.Y).FirstOrDefault();
            if (CurrentField[pos.X, pos.Y].CanEnter && p == null)
                return true;
            else return false;
        }
        public bool IsShootale(Position pos)
        {
            if (CurrentField[pos.X, pos.Y].CanShoot)
                return true;
            else return false;
        }

    }
}
