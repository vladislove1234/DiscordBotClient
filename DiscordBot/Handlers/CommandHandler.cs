using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.EventArgs;

namespace DiscordBot.Handlers
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IConfiguration _config;
        private AudioService _audioService;
        private LavaNode _lavaNode;
        public event EventHandler OnRefreshTimer;
        public event EventHandler OnRefreshFastTimer;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService commandService, IConfiguration config, LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
            _provider = provider;
            _client = client;
            _commandService = commandService;
            _config = config;
            _audioService = provider.GetRequiredService<AudioService>();
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
            _commandService.CommandExecuted += OnCommandExecuted;
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
            _client.Ready += OnReady;
            var TimerTask = new Task(async () => await Timer(5));
            var FastTimerTask = new Task(async () => await Timer(1));
            TimerTask.Start();
            _audioService.IntializeAsync(_client, _lavaNode);
        }


        private async Task OnReady()
        {
            if (!_lavaNode.IsConnected) await _lavaNode.ConnectAsync();
        }
        private async Task Timer(int TimeDelay)
        {
            if (OnRefreshTimer != null)
            OnRefreshTimer.Invoke(this, new EventArgs());
            await Task.Delay(TimeDelay * 1000);
            await Timer(TimeDelay);
        }
        private async Task FastTimer(int TimeDelay)
        {
            if (OnRefreshFastTimer != null)
                OnRefreshFastTimer.Invoke(this, new EventArgs());
            await Task.Delay(TimeDelay * 500);
            await Timer(TimeDelay);
        }
        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix("!", ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _commandService.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync($"Error: {result}");
        }
    }
}