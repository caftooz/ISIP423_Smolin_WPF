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

        public MovieDetailsPage(Movies movie)
        {
            InitializeComponent();
            _currentMovie = movie;

            this.DataContext = _currentMovie;

            LoadSessions();
        }

        private void LoadSessions()
        {
            var sessions = Core.Context.Sessions
                .Where(s => s.MovieId == _currentMovie.Id)
                .OrderBy(s => s.StartTime)
                .ToList();

            SessionsList.ItemsSource = sessions;
            
        }

        private void Session_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.DataContext is Sessions selectedSession)
            {
                if (Core.UserID == -1)
                {
                    MessageBox.Show("Для выбора мест необходимо войти в аккаунт.");
                    NavigationService.Navigate(new LoginPage());
                }
                else
                {
                    NavigationService.Navigate(new SeatSelectionPage(selectedSession));
                }
            }
        }
    }
}
