using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class TextCommands : ModuleBase<SocketCommandContext>
    {
         [Command("ping")]
         public async Task Ping(int r)
        {
            await ReplyAsync("Pong");
        }
        [Command("meme")]
        public async Task Meme()
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync("https://reddit.com/r/memes/random.json?limit=1");
            JArray arr = JArray.Parse(result);
            JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            var builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString());
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        [Command("meme")]
        public async Task MemeFromChannel([Remainder]string channel)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/" + channel + "/random.json?limit=1");
            JArray arr = JArray.Parse(result);
            JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            var builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString());
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        [Command("random")]
        public async Task Random(int min, int max)
        {
            Random rand = new Random();
            int value = rand.Next(min, max);
            await ReplyAsync(value.ToString());
        }
        [Command("random")]
        public async Task Random(int max)
        {
            Random rand = new Random();
            int value = rand.Next(max);
            await ReplyAsync(value.ToString());
        }
        [Command("coin")]
        public async Task Coin()
        {
            Random rand = new Random();
            int value = rand.Next(0, 2);
            await ReplyAsync("Кидаю монету...");
            await Task.Delay(1500);
            var builder = new EmbedBuilder();
            if (value == 1)
            {
                builder.WithImageUrl("https://www.ua-coins.info/images/coins/33_reverse.jpg");
            }
            else
            {
                builder.WithImageUrl("https://www.ua-coins.info/images/coins/33_obverse.jpg");
            }
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}
