using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Models.Entities;
using DiscordBot.Models.Games.Abstractions;
using DiscordBot.Models.Games.Enums;
using Game = DiscordBot.Models.Games.Abstractions.Game;

namespace DiscordBot.Models.Games.Tanks
{
    public class TanksGame : Game
    {
        private const int _waitingTime = 10;
        private int _waitingTimer;
        private int _enterMessageID;
        private const int _fieldWidth = 10;
        private List<Player> _players;
        private IUserMessage _gameMessage;
        private IMessage _enterMessage;
        private IMessage _moveMessage;
        public TanksGame(DiscordSocketClient client ,ITextChannel channel, IUser startUser) : base(client,channel, startUser)
        {
            _players = new List<Player>();
            _players.Add(new Player(startUser));
            State = GameState.WaitForPlayers;
            _waitingTimer = _waitingTime;
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
                _players.Add(new Player(player));
            }
            _gameMessage = await MainChannel.SendMessageAsync("Game", false, GenerateField());
        }
        protected override async Task OnMessageRecieved(SocketMessage arg)
        {
            var player = _players.Where(x => x.User.Username == arg.Author.Username).FirstOrDefault();
            if(player != null)
            {
                if (player.HasMove)
                {
                    switch (arg.Content)
                    {
                        case "w":
                            player.Position.Y -= 1;
                            break;
                        case "s":
                            player.Position.Y += 1;
                            break;
                        case "d":
                            player.Position.X += 1;
                            break;
                        case "a":
                            player.Position.X -= 1;
                            break;
                    }
                    player.HasMove = false;
                }
                await arg.DeleteAsync();
            }
        }
        protected override async Task Update()
        {
            _gameMessage = (IUserMessage)await MainChannel.GetMessageAsync(_gameMessage.Id);
            await _gameMessage.ModifyAsync(x => x.Embed = GenerateField());
            _players.ForEach(x => x.HasMove = true);
        }
        private Embed GenerateField()
        {
            var builder = new EmbedBuilder();
            string field = string.Empty;
            var rand = new Random();
            for(int x = 0; x < _fieldWidth; x++)
            {
                field = string.Empty;
                for(int y = 0; y < _fieldWidth; y++)
                {
                    var p = _players.Where(p => p.Position.X == x && p.Position.Y == y).FirstOrDefault();
                    if (p != null)
                        field += p.User.Username.ToArray()[0].ToString();
                    else
                        field += Emojies.GetNumberFromInt(rand.Next(1,5));
                }
                builder.AddField(".", field);
            }
            var build = builder.Build();
            return build;
        }
    }
}
