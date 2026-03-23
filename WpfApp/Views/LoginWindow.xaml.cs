using System.Windows;
using System.Windows.Input;
using ServiceDesk.Data;

namespace ServiceDesk.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        // PreviewKeyDown перехватывает ДО того как контрол обработает нажатие
        PreviewKeyDown += (_, e) => { if (e.Key == Key.Enter) TryLogin(); };
        TxtLogin.Focus();
    }

    private void BtnTestConn_Click(object sender, RoutedEventArgs e)
    {
        DatabaseHelper.SetConnectionString(TxtConnString.Text.Trim());
        if (DatabaseHelper.TestConnection(out string err))
            MessageBox.Show("Соединение установлено!", "OK",
                MessageBoxButton.OK, MessageBoxImage.Information);
        else
            MessageBox.Show($"Ошибка:\n{err}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void OnKeyDown(object sender, KeyEventArgs e) { }

    private void BtnLogin_Click(object sender, RoutedEventArgs e) => TryLogin();

    private void TryLogin()
    {
        TxtError.Visibility = Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(TxtLogin.Text) ||
            string.IsNullOrWhiteSpace(TxtPassword.Password))
        {
            ShowError("Введите логин и пароль.");
            return;
        }

        DatabaseHelper.SetConnectionString(TxtConnString.Text.Trim());

        try
        {
            var user = DatabaseHelper.Authenticate(
                TxtLogin.Text.Trim(), TxtPassword.Password);

            if (user == null)
            {
                ShowError("Неверный логин или пароль.");
                return;
            }

            App.CurrentUser = user;

            try
            {
                var main = new MainWindow();
                main.Show();
                Close();
            }
            catch (Exception mainEx)
            {
                MessageBox.Show(
                    $"Ошибка открытия главного окна:\n\n" +
                    $"{mainEx.Message}\n\n{mainEx.StackTrace}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка подключения к БД:\n{ex.Message}");
        }
    }

    private void ShowError(string msg)
    {
        TxtError.Text = msg;
        TxtError.Visibility = Visibility.Visible;
    }
}