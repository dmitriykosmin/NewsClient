using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;

namespace NewsClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string JsonString { get; set; }
        private List<NewsItem> News { get; set; }
        private DateTime CurrentDate { get; set; }
        private NewsView View { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            News = new List<NewsItem>();
            CurrentDate = DateTime.Now.Date;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Date.SelectedDate = CurrentDate;
            await GetNews(CurrentDate);
        }

        private async Task GetNews(DateTime date)
        {
            string query = "http://127.0.0.1:1848/News/Get";
            if (date != DateTime.Now.Date) query += "/" + date.ToString("MMddyyyy");
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(query))
                {
                    JsonString = await response.Content.ReadAsStringAsync();
                    News = NewsParser.ParseToList(JsonString).ToList();
                }
                if (View == null)
                {
                    View = new NewsView();
                }
                NewsView.SetView(News);
                UpdateView();
            }
            catch
            {
                MessageBox.Show("Соединение с сервером не установлено. Пожалуйста, проверьте ваше интернет соединение.");
            }
        }

        public void UpdateView()
        {
            SetNewsPriview();
            NewsList.ItemsSource = null;
            NewsList.ItemsSource = View;
            if (News.Count() == 0)
            {
                NoNews.Visibility = Visibility.Visible;
            }
            else
            {
                NoNews.Visibility = Visibility.Hidden;
            }
        }

        private void SetNewsPriview()
        {
            NewsPreview.Items.Clear();
            NewsPreview.Items.Add(new TextBlock(new Run("Быстрое переключение между новостями")));
            for (int i = 0; i < News.Count; i++)
            {
                Button temp = new Button()
                {
                    Content = News[i].title,
                    ClickMode = ClickMode.Release,
                    Name = "button" + i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    MinWidth = 1200
                };
                temp.Click += Preview_Click;
                NewsPreview.Items.Add(temp);
            }
            NewsPreview.SelectedIndex = 0;
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            Button temp = (Button)sender;
            int pos = Convert.ToInt32(temp.Name.Remove(0, 6));
            NewsList.Items.MoveCurrentToPosition(pos);
            NewsList.ScrollIntoView(NewsList.Items.CurrentItem);
            NewsPreview.SelectedIndex = pos + 1;
            NewsList.SelectedIndex = pos;
        }

        private async void Date_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (Date.SelectedDate != CurrentDate)
            {
                await GetNews(Date.SelectedDate.Value);
                CurrentDate = Date.SelectedDate.Value;
            }
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await GetNews(CurrentDate);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
