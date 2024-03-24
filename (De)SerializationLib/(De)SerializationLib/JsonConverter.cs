using Newtonsoft.Json;

namespace _De_SerializationLib
{
    public static class JsonConverter<T> where T : class
    {
        public static string Serialize(T obj)
        {
            var serialized = string.Empty;
            try
            {
                serialized = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return serialized;
        }

        public static T? Deserialize(string json) 
        {
            if (json == null) throw new Exception("Empty json body");
            T deserialized = null!;
            try
            {
                deserialized = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All })!;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return deserialized;
        }
    }
}
