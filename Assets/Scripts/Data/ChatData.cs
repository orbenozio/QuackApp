using Newtonsoft.Json;
using System;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class ChatData
    {
        public string Uuid;

        public string ChatName;

        public string Creator;

        public string SceneName;

        public string UserId;

        public string CreatedUTC;

        // last chat message / last chat recordings
        // members
    }
}
