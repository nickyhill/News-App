using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberNewsApp.Services
{
    public class CombinedCacheService
    {
        private readonly CacheService _memoryCache;
        private readonly FileService _fileService;

        public CombinedCacheService()
        {
            _memoryCache = new CacheService();
            _fileService = new FileService();
        }

        public async Task<T?> GetAsync<T>(string key, string fileName, Func<Task<T>> fetchDataFunc, TimeSpan memoryCacheDuration)
        {
            // 1. Check in-memory cache
            var memoryData = _memoryCache.GetFromMemory<T>(key);
            if (memoryData != null) return memoryData;

            // 2. Check file-based cache
            var fileData = await _fileService.LoadFromFileAsync<T>(fileName);
            if (fileData != null)
            {
                // Populate in-memory cache for subsequent use
                _memoryCache.SetInMemory(key, fileData, memoryCacheDuration);
                return fileData;
            }

            // 3. Fetch data from API or source
            var fetchedData = await fetchDataFunc();
            if (fetchedData != null)
            {
                // Save to both caches
                _memoryCache.SetInMemory(key, fetchedData, memoryCacheDuration);
                await _fileService.SaveToFileAsync(fileName, fetchedData);
            }

            return fetchedData;
        }

        public void ClearCache<T>(string key, string fileName)
        {
            _memoryCache.SetInMemory(key, default(T), TimeSpan.Zero); // Remove from memory
            _fileService.DeleteFile(fileName);                       // Remove from file
        }
    }

}
