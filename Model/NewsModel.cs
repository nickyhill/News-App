using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using CyberNewsApp.ViewModel;
using Microsoft.Extensions.Logging;
using System.Security.AccessControl;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;


namespace CyberNewsApp.Model
{
    public class NewsModel
    {
        public class Article : INotifyPropertyChanged
        {
            private bool _isBookmarked = false;
            public string Title { get; set; } = "[No Title]";
            public string Description { get; set; } = "[No Description]";
            public string Url { get; set; } = "[No URL]";
            public string PublishedAt { get; set; } = "[No Date]";

            public string UrlImage { get; set; } = "[No UrlImage]";

            public string Category { get; set; } = "[Category Not Set]";
            public bool IsBookmarked
            {
                get => _isBookmarked;
                set
                {
                    if (_isBookmarked != value)
                    {
                        _isBookmarked = value;
                        OnPropertyChanged(nameof(IsBookmarked));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }



        public List<Article> NewsItems { get; set; } = new List<Article>();

        public async Task<List<Article>> FetchNewsAsync(string category, string sortby)
        {

            // Fetch the API key asynchronously
            var apiKey = await GetApiKeyAsync();

            string apiUrl = $"https://newsapi.org/v2/everything?q={category}&sortBy={sortby}&apiKey={apiKey}";

            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "CyberNewsApp");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(apiUrl);
                string responseBody = await response.Content.ReadAsStringAsync();


                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Request failed with status code {response.Headers} {response.StatusCode} {response.Content}");
                }
                else
                {
                    return ParseNews(responseBody, category);
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine($"Error fetching news: {ex.Message}");
                throw new HttpRequestException($"Request failed {ex.Message}");
            }
        }

        private List<Article> ParseNews(string rawData, string category)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(rawData);
                var articles = doc.RootElement.GetProperty("articles");

                List<Article> TempNewsItems = new List<Article>();

                foreach (var article in articles.EnumerateArray())
                {

                    var title = article.GetProperty("title").GetString();
                    var description = article.GetProperty("description").GetString();
                    var url = article.GetProperty("url").GetString();
                    var publishedAt = article.GetProperty("publishedAt").GetString();
                    var urlToImage = article.GetProperty("urlToImage").GetString();

                    if (title == "[Removed]" || description == "[Removed]" || url == "[Removed]" || publishedAt == "[Removed]" || urlToImage == "[Removed]")
                    {
                        continue;
                    }

                    if (title == null || description == null || url == null || publishedAt == null || urlToImage == null)
                    {
                        continue;
                    }

                    publishedAt = FormatDate(publishedAt);

                    var newsItem = new Article
                    {
                        Title = title,
                        Description = description,
                        Url = url,
                        PublishedAt = publishedAt,
                        UrlImage = urlToImage,
                        Category = category
                    };
                    TempNewsItems.Add(newsItem);
                    Debug.WriteLine("New Article: " + newsItem.Category);
                }
                return TempNewsItems;
            }
            catch (Exception ex)
            {
                throw new JsonException($"Error parsing news data: {ex.Message}");
            }
        }

        private string FormatDate(string date)
        { 
            // Convert date from ISO 8601 to US format
            string formattedDate = "";
            if (date != null)
            {
                Debug.WriteLine("DATA: " + date);
                DateTime timeStruct;

                if (DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out timeStruct))
                {
                    formattedDate = timeStruct.ToString("hh:mm:ss tt MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    formattedDate = "Invalid Date";
                }
            }

            return formattedDate;
        }

        private async Task<string> GetApiKeyAsync()
        {
            // Open the secrets.json file as a MauiAsset
            using (var stream = await FileSystem.OpenAppPackageFileAsync("secrets.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    using JsonDocument doc = JsonDocument.Parse(json);
                    var key = doc.RootElement.GetProperty("ApiKey");
                    return key.GetString();
                }
            }
        }
    }
}
