using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class CharacterData
    {
        public string name;

        [JsonProperty("animation_controller")]
        public string animationController;

        public Dictionary<string, bool> scenes;

        [JsonProperty("animations")]
        public Dictionary<string, bool> animations;
    }
}
