using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using CyberNewsApp.Model;
using CyberNewsApp.Services;

namespace CyberNewsApp.ViewModel
{
    public class NewsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NewsModel.Article> NewsItems { get; } = new();
        public ICommand FetchNewsCommand { get; }
        public ICommand ToggleBookmarkCommand { get; }

        public ICommand ClearMainPageCommand {  get; }

        private readonly CombinedCacheService _cacheService;

        private readonly NewsModel _news;
        private readonly BookmarkModel _bookmark;
        public bool IsNotBookmarksPage => true;

        private string _category;
        private string _selectedSortBy;
        private bool _showSearchBar = true;
        private bool _loading = false;
        private bool _showArticles = false;
        private bool _showClearButton = false;

        public bool ShowSearchBar
        {
            get => _showSearchBar;
            set
            {
                if (_showSearchBar != value)
                {
                    _showSearchBar = value;
                    OnPropertyChanged(nameof(ShowSearchBar));
                }
            }
        }

        public bool ShowArticles
        {
            get => _showArticles;
            set
            {
                if (_showArticles != value)
                {
                    _showArticles = value;
                    OnPropertyChanged(nameof(ShowArticles));
                }
            }
        }

        public bool ShowClearButton
        {
            get => _showClearButton;
            set
            {
                if (_showClearButton != value)
                {
                    _showClearButton = value;
                    OnPropertyChanged(nameof(ShowClearButton));
                }
            }
        }


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

        public NewsViewModel(NewsModel news, BookmarkModel book, CombinedCacheService cacheService)
        {
            _cacheService = cacheService;
            _news = news;
            _bookmark = book;
            FetchNewsCommand = new Command<string>(async (searchText) => await FetchNewsAsync(searchText));
            ToggleBookmarkCommand = new Command<NewsModel.Article>(ToggleBookmark);
            ClearMainPageCommand = new Command(ClearMainPage);
            _cacheService = cacheService;
        }

        public async Task FetchNewsAsync(string searchText)  
        {
            var key = $"{searchText}-date";
            var fileName = $"{searchText}-date.json";

            // Ensure that the user entered a category
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Optionally, provide feedback to the user (e.g., show a message or return early)
                return;
            }



            _news.NewsItems = await _news.FetchNewsAsync(searchText, "Date");

            //await _cacheService.GetAsync<List<NewsModel.Article>>(
            //    key,
            //    fileName,
            //    async () =>
            //    {
            //        await _news.FetchNewsAsync(searchText, "Date");
            //        return _news.NewsItems;
            //    },
            //    TimeSpan.FromMinutes(30)
            //    ) ?? new List<NewsModel.Article>();

            Loading = true;
            ShowSearchBar = false;
            ShowClearButton = true;
            ShowArticles = true;
            NewsItems.Clear();
            
            foreach (var item in _news.NewsItems)
            {
                NewsItems.Add(item);
            }
            Loading = false;

        }

        public void ClearMainPage()
        {
            NewsItems.Clear();
            ShowSearchBar = true;
            ShowArticles = false;
            Loading = false;
            ShowClearButton = false;
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
