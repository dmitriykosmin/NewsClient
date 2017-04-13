using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NewsClient
{
    public class NewsView : MarkupExtension, IEnumerable
    {
        public static List<WrapPanel> Items { get; set; }
        static NewsView()
        {
            Items = new List<WrapPanel>();
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Items;
        }

        public NewsView()
        {

        }

        public static void SetView(IEnumerable<NewsItem> news)
        {
            Items.Clear();
            foreach (var item in news)
            {
                WrapPanel temp = new WrapPanel();
                temp.Orientation = Orientation.Horizontal;
                if (item.urlToImage != "")
                {
                    Image image = DownloadRemoteImageFile(item.urlToImage);
                    if (image != null)
                    {
                        temp.Children.Add(image);
                    }
                    else
                    {
                        temp.Children.Add(new TextBlock(new Run("\t\t\t\t")));
                    }
                }
                else
                {
                    temp.Children.Add(new TextBlock(new Run("\t\t\t\t")));
                }
                temp.Children.Add(new TextBlock(new Run("        ")));
                Run run = new Run("\n\n\n\n\n" + item.title);
                run.FontSize = 24;
                Hyperlink link = new Hyperlink(run);
                link.IsEnabled = false;
                link.Foreground = Brushes.DarkBlue;
                if (item.url != "")
                {
                    link.IsEnabled = true;
                    link.RequestNavigate += (sender, args) => Process.Start(args.Uri.ToString());
                    link.NavigateUri = new Uri(item.url);
                }
                TextBlock text = new TextBlock();
                text.FontSize = 24;
                text.TextAlignment = System.Windows.TextAlignment.Left;
                text.TextWrapping = System.Windows.TextWrapping.Wrap;
                text.Width = 550;
                Run addinfo = new Run("\n\n\n\n\n" + item.description);
                addinfo.FontSize = 24;
                text.Inlines.Add(link);
                text.Inlines.Add(addinfo);
                temp.Children.Add(text);
                DateTime dateTime = new DateTime();
                string time = "";
                if (DateTime.TryParse(item.publishedAt, out dateTime))
                {
                    time = dateTime.ToString();
                }
                temp.Children.Add(new TextBlock(new Run("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"
                    + item.author + "\n\n" + time)
                {
                    FontSize = 24
                })
                {
                    FontSize = 24,
                    TextWrapping = System.Windows.TextWrapping.Wrap,
                    TextAlignment = System.Windows.TextAlignment.Left,
                    Width = 220
                });
                Items.Add(temp);
            }
        }

        private static Image DownloadRemoteImageFile(string uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = 10000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if ((response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Moved ||
                    response.StatusCode == HttpStatusCode.Redirect))
                {
                    Image img = new Image();
                    BitmapImage tmp = new BitmapImage();
                    tmp.BeginInit();
                    tmp.StreamSource = response.GetResponseStream();
                    tmp.EndInit();
                    img.Source = tmp;
                    img.Height = (tmp.DpiY + 780) - tmp.DpiY;
                    img.Width = (tmp.DpiX + 650) - tmp.DpiX;
                    return img;
                }
                else throw new HttpListenerException();
            }
            catch
            {
                return null;
            }
        }
    }
}
