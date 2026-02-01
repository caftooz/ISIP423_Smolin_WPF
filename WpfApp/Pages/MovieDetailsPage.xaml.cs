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
using static System.Collections.Specialized.BitVector32;
using System.Data.Entity;

namespace WpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MovieDetailsPage.xaml
    /// </summary>
    public partial class MovieDetailsPage : Page
    {
        private Movies _currentMovie;

        // Конструктор теперь принимает объект Movie
        public MovieDetailsPage(Movies movie)
        {
            InitializeComponent();
            _currentMovie = movie;

            // Устанавливаем DataContext, чтобы в XAML работали Binding {Binding Title} и т.д.
            this.DataContext = _currentMovie;

            LoadSessions();
        }

        private void LoadSessions()
        {
            // Загружаем сеансы только для этого фильма + данные о залах
            var sessions = Core.Context.Sessions
                .Include(s => s.Halls)
                .Where(s => s.MovieId == _currentMovie.Id)
                .OrderBy(s => s.StartTime)
                .ToList();

            SessionsList.ItemsSource = sessions;
            
        }

        private void Session_Click(object sender, RoutedEventArgs e)
        {
            // Получаем сеанс, на который нажали (из Button.DataContext)
            var button = sender as Button;
            if (button.DataContext is Sessions selectedSession)
            {
                // Проверка авторизации перед переходом к местам
                if (Core.UserID == -1)
                {
                    MessageBox.Show("Для выбора мест необходимо войти в аккаунт.");
                    NavigationService.Navigate(new LoginPage());
                }
                else
                {
                    // Переходим к выбору мест, передавая выбранный сеанс
                    NavigationService.Navigate(new SeatSelectionPage(selectedSession));
                }
            }
        }
    }
}
