using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewsClient
{
    public class NewsParser
    {
        class Data
        {
            public IEnumerable<NewsItem> articles { get; set; }
        }

        public static IEnumerable<NewsItem> ParseToList(string json)
        {
            Data data = JsonConvert.DeserializeObject<Data>(json);
            return data.articles;
        }
        public static string ParseToString(IEnumerable<NewsItem> news)
        {
            return JsonConvert.SerializeObject(news);
        }
    }
}
