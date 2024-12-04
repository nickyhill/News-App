using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                Bookmarked.Add(article);

            BookmarkedChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveBookmark(NewsModel.Article article)
        {
            Bookmarked.Remove(article);
            BookmarkedChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool IsEmpty() 
        { 
            return Bookmarked.Count == 0;

        }

    }
}
