using Microsoft.Extensions.Caching.Memory;
using SQLite;
using CyberNewsApp.Helpers;
using CyberNewsApp.Model;
using System.Diagnostics;


namespace CyberNewsApp.Services
{
    public class DatabaseService
    {
        SQLiteAsyncConnection Database;

        public DatabaseService()
        {
        }

        async Task Init()
        {
            if (Database is not null)
                return;


            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await Database.CreateTableAsync<ArticleItem>();
        }

        public async Task<List<ArticleItem>> GetItemsAsync(string category, TimeSpan ttl)
        {
            Debug.WriteLine("Getting Items from DB");
            await Init();
            await DeleteExpiredItemsAsync(ttl); // Clean up expired entries
            var cutoffTime = DateTime.UtcNow - ttl;
            return await Database.Table<ArticleItem>()
                .Where(a => a.Category == category && a.CachedAt >= cutoffTime)
                .ToListAsync();
        }


        public async Task<int> SaveItemAsync(ArticleItem item)
        {
            await Init();
            if (item.ID != 0)
            {
                return await Database.UpdateAsync(item);
            }
            else
            {
                return await Database.InsertAsync(item);
            }
        }

        public async Task DeleteExpiredItemsAsync(TimeSpan ttl)
        {
            await Init();
            Debug.WriteLine("DELETING EXPIRED CACHE");
            var cutoffTime = DateTime.UtcNow - ttl;
            await Database.Table<ArticleItem>()
                          .Where(a => a.CachedAt < cutoffTime)
                          .DeleteAsync();
        }

        public async Task<int> DeleteItemAsync(ArticleItem item)
        {
            await Init();
            return await Database.DeleteAsync(item);
        }



    }
}
