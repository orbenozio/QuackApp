using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Data
{
    public class User
    {
        [JsonProperty("user_name")]
        public string Username;

        [JsonProperty("email")]
        public string Email;

        [JsonProperty("token")]
        public string Token;

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("chat_count")]
        public int ChatCount;

        [JsonProperty("active_chats")]
        public Dictionary<string, string> ActiveChats;

        [JsonProperty("invites")]
        public Dictionary<string, string> Invites;
    }
}
