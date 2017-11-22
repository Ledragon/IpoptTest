using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ipopt
{
    public static class JsonFile
    {
        private static JsonSerializerSettings SerializerSettings
        {
            get
            {
                var formatting = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.None,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.All
                };
                return formatting;
            }
        }

        public static void Save<T>(T entity, String path, Boolean indent = false, Boolean hasTypes = true)
        {
            try
            {
                var jsonSerializerSettings = SerializerSettings;
                jsonSerializerSettings.TypeNameHandling = hasTypes ? TypeNameHandling.All : TypeNameHandling.None;
                jsonSerializerSettings.Formatting = indent ? Formatting.Indented : Formatting.None;
                var serialized = JsonConvert.SerializeObject(entity, jsonSerializerSettings);
                File.WriteAllText(path, serialized, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        public static T Load<T>(String path)
        {
            try
            {
                var serialized = File.ReadAllText(path);
                var deserialized = JsonConvert.DeserializeObject<T>(serialized, SerializerSettings);
                return deserialized;
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw;
            }
        }
    }
}