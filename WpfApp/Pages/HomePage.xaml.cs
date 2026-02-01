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

            var query = Core.Context.Movies.Include(m => m.AgeRatings).AsQueryable();

            string search = SearchTextBox.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.Title.ToLower().Contains(search));
            }

            switch (SortComboBox.SelectedIndex)
            {
                case 0:
                    query = query.OrderBy(m => m.Title);
                    break;
                case 1:
                    query = query.OrderByDescending(m => m.Rating);
                    break;
            }

            MoviesListBox.ItemsSource = query.ToList();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика перехода: если не залогинен -> LoginPage, если залогинен -> ProfilePage
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
