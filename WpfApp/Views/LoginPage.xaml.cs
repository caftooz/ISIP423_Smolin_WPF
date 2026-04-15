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
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private bool _isUpdatingPhone = false; // защита от рекурсии

        public LoginPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var vm = (LoginViewModel)DataContext;
            };
        }

        private LoginViewModel VM => (LoginViewModel)DataContext;

        // ── Форматирование телефона ──────────────────────────
        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingPhone) return;
            _isUpdatingPhone = true;

            string digits = new string(PhoneBox.Text
                .Where(char.IsDigit).ToArray());

            if (digits.StartsWith("7") || digits.StartsWith("8"))
                digits = digits.Substring(1);

            if (digits.Length > 10) digits = digits.Substring(0, 10);

            string formatted = "+7 ";
            formatted += digits;

            PhoneBox.Text = formatted;
            PhoneBox.CaretIndex = formatted.Length;

            VM.PhoneNumber = digits.Length == 10
                ? "+7" + digits
                : string.Empty;

            _isUpdatingPhone = false;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
            => VM.Password = PasswordBox.Password;

        private void RegPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
            => VM.NewPassword = RegPasswordBox.Password;
    }
}
