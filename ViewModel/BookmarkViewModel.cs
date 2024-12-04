using System.Collections.ObjectModel;
using System.ComponentModel;
using CyberNewsApp.Model;

namespace CyberNewsApp.ViewModel
{

    public class BookmarkViewModel : INotifyPropertyChanged
    {
        private readonly BookmarkModel _bookmarkModel;

        
        
        public bool IsNotBookmarksPage => false;

        // Directly bind to the shared collection
        public ObservableCollection<NewsModel.Article> BookmarkItems => _bookmarkModel.Bookmarked;

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

        public void UpdateEmptyState()
        {
            // Call this method whenever BookmarkItems changes
            Empty = _bookmarkModel.IsEmpty();
        }

        public BookmarkViewModel(BookmarkModel bookmarkModel)
        {
            _bookmarkModel = bookmarkModel;

            // Subscribe to changes in the Bookmarked collection
            _bookmarkModel.BookmarkedChanged += (s, e) => UpdateEmptyState();

            // Initialize the Empty property
            UpdateEmptyState();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
