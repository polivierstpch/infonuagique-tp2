using AutoRapide.Favoris.API.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace AutoRapide.Favoris.API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        private readonly string _cacheKey = "favoris";

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public void SetValuesCacheFavoris(List<int> data)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = System.TimeSpan.FromHours(24),
                Size = data.Count()
            };
            _memoryCache.Set(_cacheKey, data, cacheEntryOptions);
        }
    }
}
