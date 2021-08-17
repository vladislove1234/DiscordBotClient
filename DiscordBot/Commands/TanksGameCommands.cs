using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Handlers;
using DiscordBot.Models.Games.Tanks;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands
{
    public class TanksGameCommands : ModuleBase<SocketCommandContext>
    {
        private DiscordSocketClient _client;
        public TanksGameCommands(DiscordSocketClient client)
        {
            _client = client;
        }
        [Command("start", RunMode = RunMode.Async)]
        public async Task Start()
        {
            using (var Game = new TanksGame(_client, (ITextChannel)Context.Channel, (IUser)Context.User))
            await Game.Execute(3.5f);
        }
    }
}
