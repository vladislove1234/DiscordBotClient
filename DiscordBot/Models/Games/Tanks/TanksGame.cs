using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Models.Entities;
using DiscordBot.Models.Games.Abstractions;
using DiscordBot.Models.Games.Enums;
using DiscordBot.Models.Games.Tanks.Fields;
using Game = DiscordBot.Models.Games.Abstractions.Game;

namespace DiscordBot.Models.Games.Tanks
{
    public class TanksGame : Game
    {
        private List<string> emojiesForPlayers; 
        private const int _waitingTime = 10;
        private int _waitingTimer;
        private int _enterMessageID;
        private const int _fieldWidth = 10;
        private List<Player> _players;
        private IUserMessage _gameMessage;
        private IMessage _enterMessage;
        private IMessage _moveMessage;
        private IUserMessage _scoreMessage;
        private List<Bullet> _bullets;
        private static string[,] map1 = new string[10, 10]
        {
            { "x","x","x","x","x","x","x","x","x","x"},
            { "x",".",".",".",".",".","w","w","w","w"},
            { "x",".","x",".","x","x","x","x","x","x"},
            { "x",".",".",".",".",".",".",".",".","x"},
            { "x",".",".",".",".",".",".",".",".","x"},
            { "x",".",".",".",".",".",".",".",".","x"},
            { "x",".",".",".",".",".",".",".",".","x"},
            { "x",".",".",".",".",".",".",".",".","x"},
            { "x",".",".",".",".","w","w","w",".","x"},
            { "x","x","x","x","x","x","x","x","x","x"}
        };
        private GameField Field = new GameField(map1,10,10);
        public TanksGame(DiscordSocketClient client ,ITextChannel channel, IUser startUser) : base(client,channel, startUser)
        {
            _players = Field.Players;
            _players.Add(new Player(startUser) { Position = GeneratePosition()});
            State = GameState.WaitForPlayers;
            _waitingTimer = _waitingTime;
            _bullets = new List<Bullet>();
        }
        protected override async Task Start()
        {
            _enterMessage = await MainChannel.SendMessageAsync("React this message to enter the game");
            await _enterMessage.AddReactionAsync(Emojies.One);
            await Task.Delay(_waitingTime * 1000);
            _enterMessage = await MainChannel.GetMessageAsync(_enterMessage.Id);
            var users = await _enterMessage.GetReactionUsersAsync(Emojies.One, 10).FlattenAsync();
            foreach (var player in users)
            {
                if(!player.IsBot)
                _players.Add(new Player(player) {Position = GeneratePosition()});
            }
            _gameMessage = await MainChannel.SendMessageAsync(GenerateField());
            _scoreMessage = await MainChannel.SendMessageAsync(GenetrateLeaderboard());
        }

        private Position GeneratePosition()
        {
            var rand = new Random();
            var spawnpos = new Position();
            while(!Field.IsMovable(spawnpos))
                spawnpos = new Position(rand.Next(0, Field.X), rand.Next(0, Field.Y));

            return spawnpos;
        }

        protected override async Task OnMessageRecieved(SocketMessage arg)
        {
            var player = _players.Where(x => x.User.Username == arg.Author.Username).FirstOrDefault();
            if(player != null)
            {
                if (player.HasMove)
                {
                    var pos = player.Position;
                    switch (arg.Content.ToCharArray()[0].ToString())
                    {
                        case "a":
                            if (Field.IsMovable(new Position(player.Position.X, player.Position.Y - 1)))
                            {
                                player.Position.Y -= 1;
                                pos.Y -= 2;
                                player.vector = VectorOfMove.Left;
                                player.HasMove = false;
                            }
                            break;
                        case "d":
                            if (Field.IsMovable(new Position(player.Position.X, player.Position.Y + 1)))
                            {
                                player.Position.Y += 1;
                                pos.Y += 2;
                                player.vector = VectorOfMove.Right;
                                player.HasMove = false;
                            }
                            break;
                        case "s":
                            if (Field.IsMovable(new Position(player.Position.X + 1, player.Position.Y)))
                            {
                                player.Position.X += 1;
                                pos.X += 2;
                                player.vector = VectorOfMove.Down;
                                player.HasMove = false;
                            }
                            break;
                        case "w":
                            if (Field.IsMovable(new Position(player.Position.X - 1, player.Position.Y)))
                            {
                                player.Position.X -= 1;
                                pos.X -= 2;
                                player.vector = VectorOfMove.Up;
                                player.HasMove = false;
                            }
                            break;
                    }
                    if (arg.Content.Contains("f"))
                    {
                        if (player.ShootPeriod <= 0)
                        {
                            var bullet = new Bullet(pos, player.vector, player);
                            _bullets.Add(bullet);
                            if (pos.X == player.Position.X && pos.Y == player.Position.Y)
                                bullet.Move();
                        }
                    }
                }
                await arg.DeleteAsync();
            }
        }
        protected override async Task Update()
        {
            _gameMessage = (IUserMessage)await MainChannel.GetMessageAsync(_gameMessage.Id);
            await _gameMessage.ModifyAsync(x => x.Content = GenerateField());
            _players.ForEach(x => { x.HasMove = true; if (x.ShootPeriod > 0) x.ShootPeriod--; });
            _bullets.ForEach(x => x.Move());
            _bullets.ForEach(x => { if (x.Position.X > Field.X && x.Position.Y > Field.Y) _bullets.Remove(x); });
            if(_players.Count <= 1)
            {
                State = GameState.End;
            }
        }
        protected override async Task End()
        {
            var bestplayer = _players[0];
            foreach(var p in _players)
            {
                if (bestplayer.Score < p.Score)
                    bestplayer = p;
            }
            _gameMessage = (IUserMessage)await MainChannel.GetMessageAsync(_gameMessage.Id);
            await _gameMessage.ModifyAsync(x => x.Content = $":medal: {bestplayer.User.Username}");
        }
        private string GenerateField()
        {
            //var builder = new EmbedBuilder();
            string field = string.Empty;
            var rand = new Random();
            for(int x = 0; x < Field.X; x++)
            {
                for(int y = 0; y < Field.Y; y++)
                {
                    var p = _players.Where(p => p.Position.X == x && p.Position.Y == y && !p.IsDead).FirstOrDefault();
                    var b = _bullets.Where(p => p.Position.X == x && p.Position.Y == y).FirstOrDefault();
                    if (p != null)
                    {
                        if (!p.IsDead)
                        {
                            if (b != null)
                            {
                                p.Kill();
                                b.Parent.AddScore(100);
                                _bullets.Remove(b);
                                field += ":boom:";
                                Task.Run(async () => { var mes = (IUserMessage)await MainChannel.GetMessageAsync(_scoreMessage.Id);
                                    await mes.ModifyAsync(x => x.Content = GenetrateLeaderboard());
                                });
                                if (_players.Where(x => !x.IsDead).Count() <= 1)
                                    State = GameState.End;
                            }
                            else
                            {
                                field += p.Emoji;
                            }
                        }
                    }
                    else
                    {
                        if (b != null)
                        {
                            if (!Field.IsShootale(b.Position))
                            {
                                _bullets.Remove(b);
                                field += Field.CurrentField[x, y].Element;
                            }
                            else
                                field += ":white_circle:";
                        }
                        else
                        {
                            field += Field.CurrentField[x,y].Element;
                        }
                    }
                }
                field += "\n";
            }
            //var build = builder.Build();
            return field;
        }
        private string GenetrateLeaderboard()
        {
            string ret = "";
            ret += "Players: \n";
            for(int i = 0; i < _players.Count; i++)
            {
                ret += $"{i + 1}.{ _players[i].Emoji} {_players[i].User.Username}  Score: {_players[i].Score} \n";
            }
            return ret;
        }
    }
}
