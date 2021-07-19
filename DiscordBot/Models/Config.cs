using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DiscordBot.Models
{
    public class Config
    {
       [JsonProperty("prefix")]
       public string prefix { get; private set; }
       [JsonProperty("token")]
       public string token { get; private set; }
    }
}
