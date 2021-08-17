namespace DiscordBot.Models.Games.Tanks
{
    public struct Position
    {
        public int X;
        public int Y;
        public Position(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }
    }
}