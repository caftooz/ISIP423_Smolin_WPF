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
        private CaptchaService _captchaService = new CaptchaService();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Если капча отображается, сначала проверяем её
            if (_captchaService.IsCaptchaRequired)
            {
                if (!_captchaService.ValidateCaptcha(CaptchaInputBox.Text))
                {
                    MessageBox.Show("Неверная капча", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    ShowCaptcha();
                    return;
                }
            }

            if (Auth(LoginBox.Text, PassBox.Password))
            {
                _captchaService.ResetFailedAttempts();
                NavigationService.Navigate(new ProfilePage());
            }
        }

        public bool Auth(string login, string password)
        {
            bool result = AuthService.Auth(login, password);
            if (!result)
            {
                _captchaService.IncrementFailedAttempts();
                MessageBox.Show("Неверный логин или пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                if (_captchaService.IsCaptchaRequired)
                {
                    ShowCaptcha();
                }
            }
            return result;
        }

        private void ShowCaptcha()
        {
            _captchaService.GenerateCaptcha();
            CaptchaPanel.Visibility = Visibility.Visible;
            CaptchaCanvas.Children.Clear();
            var captchaImage = _captchaService.RenderCaptchaImage(
                (int)CaptchaCanvas.Width > 0 ? (int)CaptchaCanvas.Width : 270,
                (int)CaptchaCanvas.Height > 0 ? (int)CaptchaCanvas.Height : 50);
            foreach (UIElement child in captchaImage.Children)
            {
                // Нужно скопировать элементы
            }
            // Заменяем содержимое канваса
            CaptchaCanvas.Children.Clear();
            var rendered = _captchaService.RenderCaptchaImage(270, 50);
            // Переносим элементы из rendered в CaptchaCanvas
            var elements = new List<UIElement>();
            foreach (UIElement el in rendered.Children)
                elements.Add(el);
            rendered.Children.Clear();
            foreach (var el in elements)
                CaptchaCanvas.Children.Add(el);

            CaptchaInputBox.Clear();
        }

        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            ShowCaptcha();
        }

        private void ToRegButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
