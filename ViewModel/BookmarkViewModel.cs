using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CyberNewsApp.Model;
using CyberNewsApp.Helpers;

namespace CyberNewsApp.ViewModel
{
    public class BookmarkViewModel : INotifyPropertyChanged
    {
        private readonly BookmarkModel _bookmarkModel;

        // Use the Grouping class for bookmark groups
        public ObservableCollection<Grouping<string, NewsModel.Article>> BookmarkItems { get; private set; }

        private bool _loading;
        private bool _empty;

        public bool Loading
        {
            get => _loading;
            set
            {
                if (_loading != value)
                {
                    _loading = value;
                    OnPropertyChanged(nameof(Loading));
                }
            }
        }

        public bool Empty
        {
            get => _empty;
            private set
            {
                if (_empty != value)
                {
                    _empty = value;
                    OnPropertyChanged(nameof(Empty));
                }
            }
        }

        public BookmarkViewModel(BookmarkModel bookmarkModel)
        {
            _bookmarkModel = bookmarkModel;
            BookmarkItems = new ObservableCollection<Grouping<string, NewsModel.Article>>();

            // Subscribe to changes in the Bookmarked collection
            _bookmarkModel.BookmarkedChanged += (s, e) => UpdateBookmarkGroups();

            // Load bookmarks when the Bookmark Page is loaded
            Task.Run(async () => await _bookmarkModel.LoadBookmarksFromFileAsync());

            // Initialize the Empty property
            UpdateEmptyState();
            UpdateBookmarkGroups();
        }

        public void RemoveBookmark(NewsModel.Article article)
        {
            _bookmarkModel.RemoveBookmark(article);
            UpdateBookmarkGroups();
        }

        public void UpdateEmptyState()
        {
            Empty = _bookmarkModel.IsEmpty();
        }

        private void UpdateBookmarkGroups()
        {
            BookmarkItems.Clear();

            // Group articles by category using your Grouping class
            var groupedArticles = _bookmarkModel.Bookmarked
                .GroupBy(article => article.Category)
                .Select(group => new Grouping<string, NewsModel.Article>(group.Key, group));

            foreach (var group in groupedArticles)
            {
                BookmarkItems.Add(group);
            }

            // Update empty state after updating groups
            UpdateEmptyState();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
