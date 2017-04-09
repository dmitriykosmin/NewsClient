using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewsClient
{
    [Serializable]
    [JsonObject]
    public class NewsItem
    {
        public string description { get; set; }
        public string publishedAt { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string urlToImage { get; set; }
    }
}
