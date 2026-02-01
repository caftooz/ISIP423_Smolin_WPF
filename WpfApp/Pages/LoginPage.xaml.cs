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

namespace WpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Context.Users.Any(u => u.Login == LoginBox.Text && u.Password == PassBox.Password))
            {
                Core.UserID = Core.Context.Users.First(u => u.Login == LoginBox.Text && u.Password == PassBox.Password).Id;
                NavigationService.Navigate(new ProfilePage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void ToRegButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
