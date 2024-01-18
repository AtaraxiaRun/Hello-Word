using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace RabbitMQ1
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

        public static string ToJson(this object t, bool HasNullIgnore)
        {
            if (HasNullIgnore)
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy-MM-dd hh:mm:ss"
                });
                return JsonConvert.SerializeObject(t, Formatting.Indented, jsonSerializerSettings);
            }

            return t.ToJson();
        }

        public static T FromJson<T>(string strJson) where T : class
        {
            if (!string.IsNullOrEmpty(strJson))
            {
                return JsonConvert.DeserializeObject<T>(strJson);
            }

            return null;
        }

        public static List<T> FromJsonList<T>(string strJson) where T : class
        {
            if (!string.IsNullOrEmpty(strJson))
            {
                return JsonConvert.DeserializeObject<List<T>>(strJson);
            }

            return null;
        }

        public static JObject Parse(string strJson)
        {
            if (string.IsNullOrEmpty(strJson))
            {
                return null;
            }

            return JObject.Parse(strJson);
        }

        public static JArray ParsestrToJArray(string strJson)
        {
            if (string.IsNullOrEmpty(strJson))
            {
                return new JArray();
            }

            return (JArray)JsonConvert.DeserializeObject(strJson);
        }

        public static string ToJson(this object obj, string datetimeformats)
        {
            IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = datetimeformats
            };
            return JsonConvert.SerializeObject(obj, isoDateTimeConverter);
        }

        public static T ToObject<T>(this string Json)
        {
            return (Json == null) ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }

        public static List<T> ToList<T>(this string Json)
        {
            return (Json == null) ? null : JsonConvert.DeserializeObject<List<T>>(Json);
        }

        public static DataTable ToTable(this string Json)
        {
            return (Json == null) ? null : JsonConvert.DeserializeObject<DataTable>(Json);
        }

        public static JObject ToJObject(this string Json)
        {
            return (Json == null) ? JObject.Parse("{}") : JObject.Parse(Json.Replace("&nbsp;", ""));
        }
    }
}
