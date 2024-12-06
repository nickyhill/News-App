using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using CyberNewsApp.Model;

namespace CyberNewsApp.ViewModel
{
    public class NewsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NewsModel.Article> NewsItems { get; } = new();
        public ICommand FetchNewsCommand { get; }
        public ICommand ToggleBookmarkCommand { get; }

        public ICommand ClearMainPageCommand {  get; }

        private readonly NewsModel _news;
        private readonly BookmarkModel _bookmark;
        public bool IsNotBookmarksPage => true;

        private string _category;
        private string _selectedSortBy;
        private bool _loading;

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

        public string Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        public string SelectedSortBy
        {
            get => _selectedSortBy;
            set
            {
                if (_selectedSortBy != value)
                {
                    _selectedSortBy = value;
                    OnPropertyChanged(nameof(SelectedSortBy));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NewsViewModel(NewsModel news, BookmarkModel book)
        {
            _news = news;
            _bookmark = book;
            FetchNewsCommand = new Command(async () => await FetchNewsAsync());
            ToggleBookmarkCommand = new Command<NewsModel.Article>(ToggleBookmark);
            ClearMainPageCommand = new Command(ClearMainPage);
        }

        public async Task FetchNewsAsync()
        {
            Loading = true;
            NewsItems.Clear();
            await _news.FetchNewsAsync(Category, SelectedSortBy);

            
            foreach (var item in _news.NewsItems)
            {
                NewsItems.Add(item);
            }
            Loading = false;
        }

        public void ClearMainPage()
        {
            NewsItems.Clear();
            Category = string.Empty;
            SelectedSortBy = string.Empty;

        }

        private void ToggleBookmark(NewsModel.Article article)
        {
            Debug.WriteLine("Bookmarked?: " + article.IsBookmarked);
            if (_bookmark.IsBookmarked(article))
            {
                _bookmark.RemoveBookmark(article);
                article.IsBookmarked = false;
            }
            else
            {
                _bookmark.AddBookmark(article);
                article.IsBookmarked = true;
            }
        }

    }
}
