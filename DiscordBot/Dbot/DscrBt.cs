using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DiscordBot.Models;
using System.Reflection;
using Victoria;
using DiscordBot.Handlers;
using DiscordBot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Discord.Addons.Hosting;

namespace DiscordBot.Dbot{
    public class DscrBt
    {
        public async Task RunAsync()
        {
            var _config = JsonConvert.DeserializeObject<Config>(jsonReturn().Result);
            var builder = new HostBuilder()
                    .ConfigureLogging(x =>
                    {
                        x.AddConsole();
                        x.SetMinimumLevel(LogLevel.Debug);
                    })
                    .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                    {
                        config.SocketConfig = new DiscordSocketConfig
                        {
                            LogLevel = LogSeverity.Verbose,
                            AlwaysDownloadUsers = true,
                            MessageCacheSize = 200,
                        };
                        config.Token = "NzY5MDg0Njg5MDI0OTQyMDgw.X5J37g.9xCm_6yOaQnaUXf_TyMbQpIokH" + "o";//Поміняти
                    })
                    .UseCommandService((context, config) =>
                    {
                        config = new CommandServiceConfig()
                        {
                            CaseSensitiveCommands = false,
                            LogLevel = LogSeverity.Verbose
                        };
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services
                        .AddSingleton<AudioService>()
                        .AddHostedService<CommandHandler>()
                        .AddSingleton<AudioService>()
                        .AddLavaNode(x =>
                        {
                            x.SelfDeaf = false;
                        });

                    })
                    .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
        private async Task<string> jsonReturn()
        {
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                return await sr.ReadToEndAsync().ConfigureAwait(false); 

        }
    }
}
