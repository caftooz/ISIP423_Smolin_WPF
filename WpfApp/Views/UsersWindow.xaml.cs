using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ServiceDesk.Data;
using ServiceDesk.Models;

namespace ServiceDesk.Views;

public partial class UsersWindow : Window
{
    public UsersWindow() { InitializeComponent(); Load(); }

    private void Load() => Grid.ItemsSource = DatabaseHelper.GetUsers();

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new UserEditDialog(null);
        if (dlg.ShowDialog() == true)
        {
            try { DatabaseHelper.SaveUser(dlg.ResultUser!, dlg.ResultPassword); Load(); }
            catch (Exception ex) { Err(ex); }
        }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not User u) { NoSel(); return; }
        var dlg = new UserEditDialog(u);
        if (dlg.ShowDialog() == true)
        {
            try { DatabaseHelper.SaveUser(dlg.ResultUser!, dlg.ResultPassword); Load(); }
            catch (Exception ex) { Err(ex); }
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not User u) { NoSel(); return; }
        if (u.Id == App.CurrentUser?.Id)
        {
            MessageBox.Show("Нельзя удалить текущего пользователя.", "Запрет",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (Confirm($"Удалить пользователя «{u.Login}»?"))
        {
            try { DatabaseHelper.DeleteUser(u.Id); Load(); }
            catch (Exception ex) { Err(ex); }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private static void NoSel() => MessageBox.Show("Выберите пользователя.", "Нет выбора",
        MessageBoxButton.OK, MessageBoxImage.Information);
    private static void Err(Exception ex) => MessageBox.Show(ex.Message, "Ошибка",
        MessageBoxButton.OK, MessageBoxImage.Error);
    private static bool Confirm(string msg) =>
        MessageBox.Show(msg, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        == MessageBoxResult.Yes;
}

// ═══════════════════════════════════════════════════════════════════════════════
//  Диалог создания / редактирования пользователя
// ═══════════════════════════════════════════════════════════════════════════════
public class UserEditDialog : Window
{
    public User?   ResultUser     { get; private set; }
    public string? ResultPassword { get; private set; }

    private readonly TextBox     _tbLogin     = new();
    private readonly PasswordBox _pbPass      = new();
    private readonly PasswordBox _pbPass2     = new();
    private readonly ComboBox    _cmbRole     = new();
    private readonly ComboBox    _cmbEmployee = new();
    private readonly CheckBox    _chkActive   = new() { Content = "Активен", IsChecked = true };

    public UserEditDialog(User? existing)
    {
        Title  = existing == null ? "Новый пользователь" : "Редактировать пользователя";
        Width  = 420; Height = 460;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;

        var inputStyle = Application.Current.Resources["InputStyle"]   as Style;
        var labelStyle = Application.Current.Resources["FormLabel"]     as Style;
        var comboStyle = Application.Current.Resources["ComboStyle"]    as Style;
        var accentBtn  = Application.Current.Resources["AccentButton"]  as Style;
        var neutralBtn = Application.Current.Resources["NeutralButton"] as Style;

        // Настройка полей
        _tbLogin.Style   = inputStyle; _tbLogin.Margin   = new Thickness(0, 0, 0, 10);
        _pbPass.FontSize  = 13; _pbPass.FontFamily  = new FontFamily("Segoe UI");
        _pbPass.Padding   = new Thickness(8, 6, 8, 6);
        _pbPass.BorderBrush = Application.Current.Resources["BorderBrush2"] as Brush;
        _pbPass.Margin    = new Thickness(0, 0, 0, 10);
        _pbPass2.FontSize  = 13; _pbPass2.FontFamily  = new FontFamily("Segoe UI");
        _pbPass2.Padding   = new Thickness(8, 6, 8, 6);
        _pbPass2.BorderBrush = Application.Current.Resources["BorderBrush2"] as Brush;
        _pbPass2.Margin    = new Thickness(0, 0, 0, 10);
        _cmbRole.Style   = comboStyle; _cmbRole.Margin   = new Thickness(0, 0, 0, 10);
        _cmbEmployee.Style = comboStyle; _cmbEmployee.Margin = new Thickness(0, 0, 0, 10);
        _chkActive.Margin = new Thickness(0, 0, 0, 18);
        _chkActive.FontSize = 13;

        // Роли
        _cmbRole.Items.Add(new ComboBoxItem { Content = "Администратор", Tag = "admin" });
        _cmbRole.Items.Add(new ComboBoxItem { Content = "Менеджер",      Tag = "manager" });
        _cmbRole.Items.Add(new ComboBoxItem { Content = "Исполнитель",   Tag = "executor" });
        _cmbRole.SelectedIndex = 1;

        // Сотрудники
        _cmbEmployee.DisplayMemberPath = "Name";
        _cmbEmployee.SelectedValuePath = "Id";
        var emps = new List<dynamic> { new { Id = (int?)null, Name = "— не привязан —" } };
        foreach (var emp in DatabaseHelper.GetEmployees())
            emps.Add(new { Id = (int?)emp.Id, Name = emp.FullName });
        _cmbEmployee.ItemsSource = emps;
        _cmbEmployee.SelectedIndex = 0;

        // Заполнение при редактировании
        if (existing != null)
        {
            _tbLogin.Text  = existing.Login;
            _chkActive.IsChecked = existing.IsActive;
            // Роль
            foreach (ComboBoxItem item in _cmbRole.Items)
                if ((string)item.Tag == existing.Role) { _cmbRole.SelectedItem = item; break; }
            // Сотрудник
            if (existing.EmployeeId.HasValue)
                foreach (var item in _cmbEmployee.Items.Cast<object>())
                {
                    dynamic d = item;
                    if (d.Id == existing.EmployeeId) { _cmbEmployee.SelectedItem = item; break; }
                }
        }

        // Метки
        TextBlock Lbl(string t) => new() { Text = t, Style = labelStyle };
        var notePass = new TextBlock
        {
            Text = existing == null ? "" : "Оставьте пустым, чтобы не менять пароль",
            FontSize = 10, Foreground = Brushes.Gray, Margin = new Thickness(0, -6, 0, 10)
        };

        // Кнопки
        var btnOk     = new Button { Content = "Сохранить", Style = accentBtn, Margin = new Thickness(0, 0, 8, 0) };
        var btnCancel = new Button { Content = "Отмена",    Style = neutralBtn };
        btnOk.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_tbLogin.Text)) { MessageBox.Show("Введите логин."); return; }
            if (existing == null && string.IsNullOrWhiteSpace(_pbPass.Password))
            { MessageBox.Show("Введите пароль."); return; }
            if (!string.IsNullOrWhiteSpace(_pbPass.Password) && _pbPass.Password != _pbPass2.Password)
            { MessageBox.Show("Пароли не совпадают."); return; }

            var role = ((ComboBoxItem)_cmbRole.SelectedItem).Tag?.ToString() ?? "manager";
            int? empId = null;
            dynamic selEmp = _cmbEmployee.SelectedItem!;
            empId = selEmp.Id;

            ResultUser = new User
            {
                Id         = existing?.Id ?? 0,
                Login      = _tbLogin.Text.Trim(),
                Role       = role,
                EmployeeId = empId,
                IsActive   = _chkActive.IsChecked == true
            };
            ResultPassword = string.IsNullOrWhiteSpace(_pbPass.Password) ? null : _pbPass.Password;
            DialogResult = true;
        };
        btnCancel.Click += (_, _) => DialogResult = false;

        var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        btnPanel.Children.Add(btnOk);
        btnPanel.Children.Add(btnCancel);

        var sp = new StackPanel { Margin = new Thickness(24) };
        sp.Children.Add(Lbl("Логин *:"));             sp.Children.Add(_tbLogin);
        sp.Children.Add(Lbl("Пароль:"));              sp.Children.Add(_pbPass);
        sp.Children.Add(Lbl("Повтор пароля:"));       sp.Children.Add(_pbPass2);
        sp.Children.Add(notePass);
        sp.Children.Add(Lbl("Роль:"));                sp.Children.Add(_cmbRole);
        sp.Children.Add(Lbl("Сотрудник:"));           sp.Children.Add(_cmbEmployee);
        sp.Children.Add(_chkActive);
        sp.Children.Add(btnPanel);

        Content = new Border
        {
            Background   = Brushes.White,
            CornerRadius = new CornerRadius(8),
            Margin       = new Thickness(16),
            Child        = sp
        };
        Background = Application.Current.Resources["LightBgBrush"] as Brush;
    }
}
