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
using WpfApp.Pages;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // При запуске открываем сразу страницу конфигуратора
            MainFrame.Navigate(new ConfiguratorPage());
        }

        private void BtnConfigurator_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ConfiguratorPage());
        }

        private void BtnMyAssemblies_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AssembliesListPage());
        }
    }
}
