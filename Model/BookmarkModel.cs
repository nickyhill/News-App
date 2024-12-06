using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using CyberNewsApp.Model;

namespace CyberNewsApp.Model
{
    public class BookmarkModel
    {
        public ObservableCollection<NewsModel.Article> Bookmarked { get; } = new();

        public event EventHandler BookmarkedChanged;

        public bool IsBookmarked(NewsModel.Article article)
        {
            return Bookmarked.Contains(article);

        }
        public void AddBookmark(NewsModel.Article article)
        {
            if (!Bookmarked.Contains(article))
            {
                Bookmarked.Insert(0,article);
                Task.Run(async () => await SaveBookmarksToFileAsync(Bookmarked));
            }
            BookmarkedChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveBookmark(NewsModel.Article article)
        {
            Bookmarked.Remove(article);
            Task.Run(async () => await SaveBookmarksToFileAsync(Bookmarked));
            BookmarkedChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsEmpty()
        {
            return Bookmarked.Count == 0;

        }

        // Get and Save Booksmarks locally

        private string GetBookmarkFilePath()
        {
            return Path.Combine(FileSystem.Current.AppDataDirectory, "bookmarks.json");
        }

        public async Task SaveBookmarksToFileAsync(IEnumerable<NewsModel.Article> bookmarks)
        {
            string json = JsonSerializer.Serialize(bookmarks);
            string filePath = GetBookmarkFilePath();
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task LoadBookmarksFromFileAsync()
        {
            string filePath = GetBookmarkFilePath();
            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                var loadedBookmarks = JsonSerializer.Deserialize<List<NewsModel.Article>>(json);

                // Clear and add items to the observable collection
                Bookmarked.Clear();
                if (loadedBookmarks != null)
                {
                    foreach (var bookmark in loadedBookmarks)
                    {
                        Bookmarked.Add(bookmark);
                    }
                    BookmarkedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }


    }
}
