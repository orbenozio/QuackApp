using Newtonsoft.Json;
using System;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class ChatData
    {
        [JsonProperty("uuid")]
        public string Uuid;

        [JsonProperty("chat_name")]
        public string ChatName;

        [JsonProperty("creator")]
        public string Creator;

        [JsonProperty("scene_name")]
        public string SceneName;

        [JsonProperty("user_id")]
        public string UserId;

        [JsonProperty("created_utc")]
        public string CreatedUTC;

        // last chat message / last chat recordings
        // members
    }
}
