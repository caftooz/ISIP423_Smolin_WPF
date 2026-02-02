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
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(EmailBox.Text) || String.IsNullOrEmpty(LoginBox.Text) ||
                String.IsNullOrEmpty(PassBox.Password) || String.IsNullOrEmpty(PassRepBox.Password))
            {
                MessageBox.Show("Все поля должны быть заполнены");
                return;
            }
            if (Core.Context.Users.Any(u => u.Email == EmailBox.Text))
            {
                MessageBox.Show("Пользователь с такой почтой уже существует");
                return;
            }
            if (Core.Context.Users.Any(u => u.Login == LoginBox.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
                return;
            }
            if (PassBox.Password != PassRepBox.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }
            Users newUser = new Users() 
            { 
                Login = LoginBox.Text,
                Email = EmailBox.Text,
                Password = PassBox.Password
            };
            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();

            Core.UserID = newUser.Id;
            NavigationService.Navigate(new HomePage());
        }
    }
}
