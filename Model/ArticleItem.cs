using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberNewsApp.Model
{
    public class ArticleItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string PublishedAt { get; set; }

        public string UrlImage { get; set; }

        [Indexed]
        public string Category { get; set; }

        public DateTime CachedAt { get; set; }

    }
}
