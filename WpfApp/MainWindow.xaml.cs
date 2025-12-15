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

namespace WpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<CarModel> CarModels { get; set; }
        public MainWindow()
        {
            CarModels = new List<CarModel>()
            {
                new CarModel("BMW", 300),
                new CarModel("Audi", 100),
                new CarModel("Lamborghini", 10),
            };

            InitializeComponent();

            DataContext = this;
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoForward)
            {
                MainFrame.GoForward();
            }
        }

        private void MainFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            BackButton.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            ForwardButton.Visibility = MainFrame.CanGoForward ? Visibility.Visible : Visibility.Collapsed;

            if (e.Content is Page page)
            {
                TitleTextBlock.Text = page.Title;
            }
        }
    }
}
