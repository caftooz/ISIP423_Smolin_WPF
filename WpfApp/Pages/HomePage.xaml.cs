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
using System.Data.Entity;

namespace WpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (SearchTextBox == null || SortComboBox == null || MoviesListBox == null) return;

            var movies = Core.Context.Movies.AsQueryable();

            string search = SearchTextBox.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
            {
                movies = movies.Where(m => m.Title.ToLower().Contains(search));
            }

            switch (SortComboBox.SelectedIndex)
            {
                case 0:
                    movies = movies.OrderBy(m => m.Title);
                    break;
                case 1:
                    movies = movies.OrderByDescending(m => m.Rating);
                    break;
            }

            MoviesListBox.ItemsSource = movies.ToList();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (Core.UserID == -1)
                NavigationService.Navigate(new LoginPage());
            else  
                NavigationService.Navigate(new ProfilePage());
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void MoviesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MoviesListBox.SelectedItem is Movies selectedMovie)
            {
                NavigationService.Navigate(new MovieDetailsPage(selectedMovie));
            }
        }
    }
}
