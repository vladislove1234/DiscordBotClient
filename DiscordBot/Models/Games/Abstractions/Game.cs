using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Models.Games.Enums;

namespace DiscordBot.Models.Games.Abstractions
{
    public abstract class Game : IDisposable
    {
        public ITextChannel MainChannel { get; protected set; }
        public IUser StartUser { get; protected set; }
        public GameState State { get; protected set; }
        public DiscordSocketClient _client { get; protected set; }

        public Game(DiscordSocketClient client,ITextChannel channel, IUser startUser)
        {
            MainChannel = channel;
            StartUser = startUser;
            _client = client;
            _client.MessageReceived += OnMessageRecieved;
        }
        protected virtual async Task Start()
        {

        }
        protected virtual async Task End()
        {

        }
        public async Task Execute(float Period)
        {
            await Start();
            while(State != GameState.End)
            {
                await Task.Delay((int)(Period * 1000));
                await Update();
            }
            await End();
        }
        protected virtual async Task OnMessageRecieved(SocketMessage arg)
        {
            
        }

        protected virtual async Task Update()
        {

        }

        public virtual void Dispose()
        {
            _client.MessageReceived -= OnMessageRecieved;
        }
    }
}
