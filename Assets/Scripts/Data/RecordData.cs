using System;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class RecordData
    {
        public float[] ClipSamples;
        public string UserId;
        public string ChatRoomId;
        public string ChatCharacterKey;
        public string CreatedUTC;
    }
}
