using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum020.Service.SessionHelper
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
            session.Set(key, data);
        }

        public static bool TryGetObject<T>(this ISession session, string key, out T value)
        {
            if (session.TryGetValue(key, out var data))
            {
                value = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
                return true;
            }
            value = default;
            return false;
        }
    }
}
