using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class SceneData
    {
        public int id;
        public string image;
        public bool isActive;
        public string name;
        public Dictionary<string, bool> characters;
    }
}
