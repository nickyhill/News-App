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



        public List<Article> NewsItems { get; private set; } = new List<Article>();

        public async Task FetchNewsAsync(string category, string sortby)
        {
            string apiUrl = $"https://newsapi.org/v2/everything?q={category}&sortBy={sortby}&apiKey=4f2dfe5f34d4427f949b18afecf1f636";

            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "CyberNewsApp");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(apiUrl);
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseBody);

               
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Request failed with status code {response.Headers} {response.StatusCode} {response.Content}");
                }
                else
                {
                   ParseNews(responseBody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching news: {ex.Message}");
            }
        }

        private void ParseNews(string rawData)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(rawData);
                var articles = doc.RootElement.GetProperty("articles");

                NewsItems.Clear();

                foreach (var article in articles.EnumerateArray())
                {

                    var title = article.GetProperty("title").GetString();
                    var description = article.GetProperty("description").GetString();
                    var url = article.GetProperty("url").GetString();
                    var publishedAt = article.GetProperty("publishedAt").GetString();

                    if (title == "[Removed]" || description == "[Removed]" || url == "[Removed]" || publishedAt == "[Removed]")
                    {
                        continue;
                    }

                    if (title == null || description == null || url == null || publishedAt == null)
                    {
                        continue;
                    }

                    publishedAt = FormatDate(publishedAt);

                    var newsItem = new Article
                    {
                        Title = title,
                        Description = description,
                        Url = url,
                        PublishedAt = publishedAt
                    };
                    NewsItems.Add(newsItem);
                    Debug.WriteLine("New Article: " + newsItem);
                }
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
    }
}
