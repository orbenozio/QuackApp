using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Data
{
    public class User
    {
        public string Name;

        public string Email;

        public string Token;

        public string Id;

        public int ChatCount;

        public Dictionary<string, string> ActiveChats;

        public Dictionary<string, string> Invites;
    }
}
