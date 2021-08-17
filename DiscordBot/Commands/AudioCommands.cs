using System.Threading.Tasks;
using DiscordBot.Services;
using Discord;
using Discord.Commands;
using Victoria;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Models.Entities;

namespace DiscordBot.Commands
{
    public sealed class AudioCommands : ModuleBase<SocketCommandContext>
    {
        private LavaNode _lavaNode;
        private AudioService _audioService;
        public AudioCommands(LavaNode lavaNode, IServiceProvider provider)
        {
            _lavaNode = lavaNode;
            _audioService = provider.GetRequiredService<AudioService>();
        }
        [Command("Leave")]
        public async Task LeaveAsync()
        {
            await ReplyAsync(_audioService.LeaveAsync(Context.User as IVoiceState, Context.Guild).Result);
        }
        [Command("Join")]
        public async Task JoinAsync()
        {
            await ReplyAsync(_audioService.JoinAsync(Context.User as IVoiceState, Context.Guild, Context.Channel as ITextChannel).Result);
        }
        [Command("Play")]
        public async Task PlayAsync([Remainder] string query)
        {
            await ReplyAsync(_audioService.PlayAsync(query, Context.Guild).Result);
        }
        [Command("SkipRange")]
        public async Task SkipRangeAsync([Remainder] int count)
        {
            await ReplyAsync(_audioService.SkipRangeAsync(count,Context.Guild).Result);
        }
        [Command("Skip")]
        public async Task SkipAsync()
        {
            await ReplyAsync(_audioService.SkipAsync(Context.Guild).Result);
        }
        [Command("Stop")]
        public async Task StopAsync()
        {
            await ReplyAsync(_audioService.StopAsync(Context.Guild).Result);
        }
        [Command("Pause")]
        public async Task PauseAsync()
        {
            await ReplyAsync(_audioService.PauseAsync(Context.Guild).Result);
        }
        [Command("Playlist")]
        public async Task PlaylistAsync([Remainder] string searchQuery)
        {
            await ReplyAsync(_audioService.PlaylistAsync(searchQuery, Context.Guild).Result);
        }
        [Command("InsertTrack")]
        public async Task InsertTrackAsync([Remainder] string searchQuery)
        {
            await ReplyAsync(_audioService.InsertTrackAsync(searchQuery,Context.Guild).Result);
        }
        [Command("List")]
        public async Task ReturnList()
        {

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync(":warning: I'm not connected to a voice channel! :warning:");
            }
            else
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                player.Queue.ToList().ForEach(x => Console.WriteLine($"{x.Title}\n"));
            }
        }
        [Command("Search")]
        public async Task SearchASync([Remainder] string search)
        {
           var Tracks =  _audioService.SearchAsync(search, Context.Guild, (ITextChannel)Context.Channel).Result;

           if(Tracks != null)
            {
                var builder = new EmbedBuilder()
                .AddField(":one:", $"**{Tracks[0].Title}** ```{Tracks[0].Duration}```", true)
                .AddField(":two:", $"**{Tracks[1].Title}** ```{Tracks[1].Duration}```", true)
                .AddField(":three:", $"**{Tracks[2].Title}** ```{Tracks[2].Duration}```", true)
                .AddField(":four:", $"**{Tracks[3].Title}** ```{Tracks[3].Duration}```", true)
                .AddField(":five:", $"**{Tracks[4].Title}** ```{Tracks[4].Duration}```",true);
                var embed = builder.Build();
                var message = await Context.Channel.SendMessageAsync(null,false, embed);

                await message.AddReactionsAsync(new Emoji[] { Emojies.One, Emojies.Two, Emojies.Three, Emojies.Four, Emojies.Five });

                await Task.Delay(20000);

                var Reactionsmessages = await Context.Channel.GetMessageAsync(message.Id);
                int takenSong = 0;
                Reactionsmessages.Reactions.TryGetValue(Emojies.One, out var val1);
                Reactionsmessages.Reactions.TryGetValue(Emojies.Two, out var val2);
                Reactionsmessages.Reactions.TryGetValue(Emojies.Three, out var val3);
                Reactionsmessages.Reactions.TryGetValue(Emojies.Four, out var val4);
                Reactionsmessages.Reactions.TryGetValue(Emojies.Five, out var val5);
                //int UsersOne = Newmessage.GetReactionUsersAsync( Emojies.One, 100).ToListAsync().Result.Count();
                //int UsersTwo = Newmessage.GetReactionUsersAsync(Emojies.Two, 100).ToListAsync().Result.Count();
                //int UsersThree = Newmessage.GetReactionUsersAsync(Emojies.Three, 100).ToListAsync().Result.Count();
                //int UsersFour = Newmessage.GetReactionUsersAsync(Emojies.Four, 100).ToListAsync().Result.Count();
                //int UsersFive = Newmessage.GetReactionUsersAsync(Emojies.Five, 100).ToListAsync().Result.Count();
                int[] UsersChoose = { val1.ReactionCount,
                    val2.ReactionCount ,
                    val3.ReactionCount,
                    val4.ReactionCount,
                    val5.ReactionCount};
                for(int i = 1; i < 5; i++)
                {
                    if (UsersChoose[i] > UsersChoose[takenSong]) takenSong = i;
                }
                await ReplyAsync(_audioService.PlayTrack(Tracks[takenSong], Context.Guild).Result);
            }

        }
    }
}