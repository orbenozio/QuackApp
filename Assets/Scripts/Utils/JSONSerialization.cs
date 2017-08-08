using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utils
{
    [Serializable]
    public class JSONSerialization<T>
    {
        public T value;

        public static T CreateFromJSON(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static string CreateFromObject(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
   
        public static Dictionary<string, T> CreateDictionaryFromJSON(string json)
        {
            string newJson = "{ \"dict\": " + json + "}";
            Wrapper<T> wrapper = JSONSerialization<Wrapper<T>>.CreateFromJSON(newJson);
            //Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.dict;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public Dictionary<string, T> dict;
        }
    }
}
