using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using CyberNewsApp.Model;
using CyberNewsApp.Services;
using Microsoft.Extensions.Caching.Memory;

namespace CyberNewsApp.ViewModel
{
    public class NewsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NewsModel.Article> NewsItems { get; } = new();
        public ICommand FetchNewsCommand { get; }
        public ICommand ToggleBookmarkCommand { get; }

        public ICommand ClearMainPageCommand {  get; }

        private readonly IMemoryCache _cacheService;

        private readonly NewsModel _news;
        private readonly BookmarkModel _bookmark;
        public bool IsNotBookmarksPage => true;

        DatabaseService _db;

        private string _category;
        private string _selectedSortBy;
        private bool _showSearchBar = true;
        private bool _loading = false;
        private bool _showArticles = false;
        private bool _showClearButton = false;



        #region Visibility_Bools

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

#endregion




        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NewsViewModel(NewsModel news, BookmarkModel book, DatabaseService Database, IMemoryCache cacheService)
        {
            _cacheService = cacheService;
            _news = news;
            _bookmark = book;
            _db = Database;
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


            // -------------------  Caching Logic  -----------------------------------------

            if (!_cacheService.TryGetValue(key, out List<NewsModel.Article>? articles))
            {
                var ttl = TimeSpan.FromMinutes(60);
                var dbItems = await _db.GetItemsAsync(searchText, ttl);

                if (dbItems.Any())
                {
                    Debug.WriteLine(dbItems.Count());
                    Debug.WriteLine($"Database hit for category: {searchText}");
                    articles = ConvertToArticles(dbItems);
                }
                else
                {
                    Debug.WriteLine($"Fetching data for category: {searchText} from API.");
                    _news.NewsItems = await _news.FetchNewsAsync(searchText, "Date");

                    await SaveArticlesAsync(ConvertToArticleItems(_news.NewsItems), searchText);
                    articles = _news.NewsItems;
                }

                // Store in memory cache
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };
                _cacheService.Set(key, articles, cacheOptions);
                _news.NewsItems = articles;
            }

            else
            {
                Debug.WriteLine($"cache hit. fetching data for key: {key} from cache.");
                _news.NewsItems = articles;
            }

            //  -------------------  End Caching Logic  -----------------------------------------

            var sortNews = _news.NewsItems
                            .OrderByDescending(item => DateTime.Parse(item.PublishedAt, CultureInfo.InvariantCulture))
                            .ToList();


            Loading = true;
            ShowSearchBar = false;
            ShowClearButton = true;
            ShowArticles = true;
            NewsItems.Clear();
            
            foreach (var item in sortNews)
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

        #region Database_Helpers
        private List<NewsModel.Article> ConvertToArticles(List<ArticleItem> items)
        {
            return items.Select(item => new NewsModel.Article
            {
                Title = item.Title,
                Description = item.Description,
                Url = item.Url,
                PublishedAt = item.PublishedAt,
                UrlImage = item.UrlImage,
                Category = item.Category
            }).ToList();
        }

        private List<ArticleItem> ConvertToArticleItems(List<NewsModel.Article> articles)
        {
            return articles.Select(article => new ArticleItem
            {
                Title = article.Title,
                Description = article.Description,
                Url = article.Url,
                PublishedAt = article.PublishedAt,
                UrlImage = article.UrlImage,
                Category = article.Category
            }).ToList();
        }


        public async Task SaveArticlesAsync(List<ArticleItem> articles, string category)
        {
            foreach (var article in articles)
            {
                article.Category = category;
                article.CachedAt = DateTime.UtcNow;
                await _db.SaveItemAsync(article);
            }
        }

        #endregion

    }
}
