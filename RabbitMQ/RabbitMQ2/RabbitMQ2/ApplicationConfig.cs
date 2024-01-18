using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ2
{
    /// <summary>
    /// 静态类配置
    /// </summary>
    public static class ApplicationConfig
    {
        public static IConfiguration Configuration = null;

        private static ConcurrentDictionary<string, List<string>> _dicCache = new ConcurrentDictionary<string, List<string>>();

        public static void SetConfiguration(IConfiguration configuration)
        {
            if (Configuration == null)
            {
                Configuration = configuration;
            }
        }

        public static string GetValue(string key)
        {
            _dicCache.TryGetValue(key, out var value);
            if (value != null)
            {
                return value?[0];
            }

            if (Configuration == null)
            {
                return "";
            }

            string text = Configuration[key];
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }

            return "";
        }

        public static List<string> GetReferenceValue(string key)
        {
            _dicCache.TryGetValue(key, out var value);
            if (value == null)
            {
                value = new List<string> { GetValue(key) };
                _dicCache.TryAdd(key, value);
            }

            return value;
        }
    }
}
