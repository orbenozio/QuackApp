using Newtonsoft.Json;
using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    public class User
    {
        public string FirstName;

        public string LastName;

        public string Username;

        public string Email;

        public string Token;

        public string Id;

        public int ChatCount;

        public Dictionary<string, string> ActiveChats;

        public Dictionary<string, string> Invites;
    }
}
