using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Models;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace DiscordBot.Services
{
    public class AudioService
    {
        private DiscordSocketClient _client;
        private LavaNode _lavaNode;
        public IGuild Guild = null;
        public int CountOfplayers = -1;
        public LavaTrack UserEnterTrack; // задавати через бд
        public async void IntializeAsync(DiscordSocketClient client, LavaNode lavaNode)
        {
            _client = client;
            _lavaNode = lavaNode;
            _lavaNode.OnTrackEnded += OnTrackFinished;
            _client.UserVoiceStateUpdated += OnUserEntered;
            SetStartMusic();
        }
        public async void SetStartMusic()
        {
            var searchResponse = await _lavaNode.SearchYouTubeAsync("Добро пожаловать на сервер Шизофрения!");
            if (searchResponse.LoadStatus != LoadStatus.LoadFailed ||
                searchResponse.LoadStatus != LoadStatus.NoMatches)
            {
                UserEnterTrack = searchResponse.Tracks[0];
            }
            else
            {
                Console.WriteLine("Failed to get UserEnter music");
                UserEnterTrack = null;
            }
        }
        private async Task OnUserEntered(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            if (Guild != null && UserEnterTrack != null)
            {
                if (_lavaNode.HasPlayer(Guild))
                {
                    var player = _lavaNode.GetPlayer(Guild);
                    var Users = player.VoiceChannel.GetUsersAsync();
                    if (CountOfplayers == -1)
                    {
                        CountOfplayers = Users.CountAsync().Result;
                    }
                    else
                    {
                        if (arg3.VoiceChannel != null)
                        {
                            if (arg3.VoiceChannel == player.VoiceChannel)
                            {
                                if (CountOfplayers < Users.CountAsync().Result)
                                {
                                    CountOfplayers = Users.CountAsync().Result;
                                    if (player.Track != UserEnterTrack)
                                    {
                                        if (player.PlayerState == PlayerState.Playing)
                                        {
                                            var Currtrack = player.Track;
                                            await player.PauseAsync();
                                            await player.PlayAsync(UserEnterTrack);
                                            await player.ResumeAsync();
                                        }
                                        else
                                        {
                                            await player.PlayAsync(UserEnterTrack);
                                        }
                                    }
                                }
                            }
                        }
                        else CountOfplayers--;
                    }
                }
            }
            else Console.WriteLine($"Guild = {Guild is null} \n Track = {UserEnterTrack is null}");
        }
        private async Task OnTrackFinished(TrackEndedEventArgs args)
        {
            if (!args.Reason.ShouldPlayNext())
            {
                return;
            }

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable))
            {
                if(player.Track != UserEnterTrack)
                {
                    await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks to rock n' roll!");
                    return;
                }
                else return;
            }

            if (!(queueable is LavaTrack track))
            {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(track);
            await args.Player.TextChannel.SendMessageAsync(
                $" :notes: ***Now playing***: {track.Title}");
        }
        public async Task<string> LeaveAsync(IVoiceState voiceState, IGuild guild)
        {
            var _voiceState = voiceState;
            if (voiceState.VoiceChannel == null)
            {
                return ":warning:You must be connected to a voice channel!. :warning:";
            }
            else if (!_lavaNode.HasPlayer(guild))
            {
                return ":warning:I'm not connected to a voice channel!. :warning:";
            }
            else
            {
                await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
                Guild = null;
                return $"Player left {voiceState.VoiceChannel.Name}";
            }
        }
        public async Task<string> JoinAsync(IVoiceState voiceState, IGuild guild, ITextChannel textChannel)
        {
            var _voiceState = voiceState;
            if (_lavaNode.HasPlayer(guild))
            {
                return $"I'm already connected to {voiceState.VoiceChannel.Name}";
            }

            if (voiceState?.VoiceChannel == null)
            {
                return ":warning:You must be connected to a voice channel!. :warning:";
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, textChannel);
                Guild = guild;
                return $"Joined {voiceState.VoiceChannel.Name}!";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }
        public async Task<string> PlayAsync(string query, IGuild guild)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return ":warning:Please provide search terms. :warning:";
            }

            if (!_lavaNode.HasPlayer(guild))
            {
                return ":warning: I'm not connected to a voice channel. :warning:";
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                return $"  ***I wasn't able to find anything for*** `{query}`.";
            }

            var player = _lavaNode.GetPlayer(guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                var track = searchResponse.Tracks[0];
                player.Queue.Enqueue(track);
                return $"*** :notes: Enqueued***: {track.Title}";

            }
            else
            {
                var track = searchResponse.Tracks[0];
                await player.PlayAsync(track);
                return $"*** :notes: Now Playing:*** {track.Title}";
            }

        }
        public async Task<string> PlayTrack (LavaTrack track, IGuild guild)
        {
            if(track != null)
            {
                var player = _lavaNode.GetPlayer(guild);
                if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
                {
                    player.Queue.Enqueue(track);
                    return $"*** :notes: Enqueued***: {track.Title}";

                }
                else
                {
                    await player.PlayAsync(track);
                    return $"*** :notes: Now Playing:*** {track.Title}";
                }
            }
            return ":warning:Failed to play track:warning:";
        }
        public async Task<string> SkipRangeAsync(int count,IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (!_lavaNode.HasPlayer(guild))
            {
                return ":warning: I'm not connected to a voice channel. :warning:";
            }
            else if (player.Queue.Count > count - 1)
            {
                player.Queue.RemoveRange(0, count);
                return $"Skipped {count} tracks";
            }
            else
            {
                await player.StopAsync();
                return "All tracks are finished!";
            }

        }
        public async Task<string> SkipAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (!_lavaNode.HasPlayer(guild))
            {
                return ":warning: I'm not connected to a voice channel. :warning:";
            }
            else if (player.Queue.Count > 0)
            {
                await player.SkipAsync();
                return $"Next Track : {player.Track.Title}";
            }
            else
            {
               await player.StopAsync();
               return "All tracks are finished!";
            }

        }
        public async Task<string> StopAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (!_lavaNode.HasPlayer(guild))
            {
                return ":warning: I'm not connected to a voice channel. :warning:";
            }
            else
            {
                await player.StopAsync();
                return "Player has been stoped";
            }
        }
        public async Task<string> PauseAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (!_lavaNode.HasPlayer(guild))
            {
                return ":warning: I'm not connected to a voice channel. :warning:";
            }
            else if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Paused)
            {
                await player.ResumeAsync();
                return "Playing is resumed!";
            }
            else
            {
                await player.PauseAsync();
                return "Playing is paused!";
                
            }

        }
        public async Task<string> PlaylistAsync(string searchQuery, IGuild guild)
        {
            string Retstr = "";
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return ":warning:Please provide search terms. :warning:";
            }

            if (!_lavaNode.HasPlayer(guild))
            {
                return ":warning: I'm not connected to a voice channel. :warning:";
            }

            var queries = searchQuery.Split(' ');
            foreach (var query in queries)
            {
                var searchResponse = await _lavaNode.SearchAsync(query);
                if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                    searchResponse.LoadStatus == LoadStatus.NoMatches)
                {
                    Retstr += $"I wasn't able to find anything for `{query}`.\n";
                }

                var player = _lavaNode.GetPlayer(guild);

                if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
                {
                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                    {
                        foreach (var track in searchResponse.Tracks)
                        {
                            player.Queue.Enqueue(track);
                        }

                        Retstr += $"Enqueued {searchResponse.Playlist.Name}\n";
                    }
                    else
                    {
                        var track = searchResponse.Tracks[0];
                        player.Queue.Enqueue(track);
                        Retstr += $"Enqueued: {track.Title}\n";
                    }
                }
                else
                {
                    var track = searchResponse.Tracks[0];

                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                    {
                        for (var i = 0; i < searchResponse.Tracks.Count; i++)
                        {
                            if (i == 0)
                            {
                                await player.PlayAsync(track);
                                Retstr += $"Now Playing: {track.Title}\n";
                            }
                            else
                            {
                                player.Queue.Enqueue(searchResponse.Tracks[i]);
                            }
                        }

                        Retstr += $"Enqueued {searchResponse.Playlist.Name}\n";
                    }
                    else
                    {
                        await player.PlayAsync(track);
                        Retstr += $"Now Playing: {track.Title}\n";
                    }
                }
            }
            return Retstr;
        }
        public async Task<string> InsertTrackAsync(string searchQuery, IGuild guild)
        {
            string Retstr = "";
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return ":warning: :warning:Please provide search terms. :warning: :warning:";
            }

            if (!_lavaNode.HasPlayer(guild))
            {
                return ":warning: :warning: I'm not connected to a voice channel. :warning: :warning:";
            }

            var queries = searchQuery.Split(' ');
            foreach (var query in queries)
            {
                var searchResponse = await _lavaNode.SearchAsync(query);
                if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                    searchResponse.LoadStatus == LoadStatus.NoMatches)
                {
                    Retstr += $"I wasn't able to find anything for `{query}`.\n";
                }

                var player = _lavaNode.GetPlayer(guild);

                if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
                {
                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                    {
                        foreach (var track in searchResponse.Tracks)
                        {
                            player.Queue.ToList().Insert(1,track);
                        }

                        Retstr += $"Inserted {searchResponse.Playlist.Name}\n";
                    }
                    else
                    {
                        var track = searchResponse.Tracks[0];
                        player.Queue.ToList().Insert(1, track);
                        Retstr += $"Inserted: {track.Title}\n";
                    }
                }
                else
                {
                    var track = searchResponse.Tracks[0];

                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                    {
                        for (var i = 0; i < searchResponse.Tracks.Count; i++)
                        {
                            if (i == 0)
                            {
                                await player.PlayAsync(track);
                                Retstr += $"Now Playing: {track.Title}\n";
                            }
                            else
                            {
                                player.Queue.ToList().Insert(1, track);
                            }
                        }

                        Retstr += $"Inserted {searchResponse.Playlist.Name}\n";
                    }
                    else
                    {
                        await player.PlayAsync(track);
                        Retstr += $"Now Playing: {track.Title}\n";
                    }
                }
            }
            return Retstr;
        }
        public async Task<List<LavaTrack>> SearchAsync (string search, IGuild guild, ITextChannel channel)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                await channel.SendMessageAsync(":warning: Please provide search terms. :warning: ");
                return null;
            }
            if (!_lavaNode.HasPlayer(guild))
            {
                await channel.SendMessageAsync(":warning: I'm not connected to a voice channel. :warning:");
                return null;
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(search);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await channel.SendMessageAsync($"  ***I wasn't able to find anything for*** `{ search }`.");
                return null;
            }
            return searchResponse.Tracks.Take(5).ToList();
        }
    }
}

    
       
    


