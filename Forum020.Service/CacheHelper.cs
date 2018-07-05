using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum020.Service.CacheHelper
{
    public static class CachingExtensions
    {
        public static async Task SetObjectAsync<T>(
            this IDistributedCache cache, string key, T value)
        {
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(value));
        }

        public static async Task<T> GetObjectAsync<T>(
            this IDistributedCache cache, string key)
        {
            var value = await cache.GetStringAsync(key);
            return value == null ? default :
                                  JsonConvert.DeserializeObject<T>(value);
        }
    }
}
