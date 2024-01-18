using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ2
{
    public static class JsonHelper
    {
        public static string ToJson(this object t)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include
            };
            jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd hh:mm:ss"
            });
            return JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);
        }

        public static string ToJson<T>(this T t)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include
            };
            jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd hh:mm:ss"
            });
            return JsonConvert.SerializeObject(t, Formatting.Indented, jsonSerializerSettings);
        }
        public static T ToObject<T>(this string Json)
        {
            return (Json == null) ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }


    }
}
