using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class ChatCharacterData
    {
        public string UserId;
        public string Key;
        public string CharacterId;
        public Vector2 Position;
        public string ChatRoomId;
        public CharacterData Data;
    }
}
