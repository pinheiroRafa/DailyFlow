

using AuthApi.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ConsolidatedAPI.Services 
{
    public class CacheService(ConnectionMultiplexer connection) : ICacheService
    {

        private readonly IDatabase _database = connection.GetDatabase();

        public async Task<bool> Create<T>(string key, int minutes, T value)
        {
            var serialized = JsonConvert.SerializeObject(value);
            await _database.StringSetAsync(key, serialized, TimeSpan.FromMinutes(minutes));
            return true;
        }

        public async Task<T?> Find<T>(string key)
        {
            var cachedData = await _database.StringGetAsync(key);
            if (cachedData.IsNullOrEmpty)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(cachedData!);
        }
    }
}