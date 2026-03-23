using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ServiceDesk.Data;
using ServiceDesk.Models;

namespace ServiceDesk.Views.Directories;

public partial class EmployeesWindow : Window
{
    public EmployeesWindow() { InitializeComponent(); Load(); }

    private void Load() => Grid.ItemsSource = DatabaseHelper.GetEmployees();

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new EmployeeEditDialog(null);
        if (dlg.ShowDialog() == true)
        {
            try { DatabaseHelper.SaveEmployee(dlg.Result!); Load(); }
            catch (Exception ex) { Err(ex); }
        }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not Employee emp) { NoSel(); return; }
        var dlg = new EmployeeEditDialog(emp);
        if (dlg.ShowDialog() == true)
        {
            try { DatabaseHelper.SaveEmployee(dlg.Result!); Load(); }
            catch (Exception ex) { Err(ex); }
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not Employee emp) { NoSel(); return; }
        if (Confirm($"Удалить сотрудника «{emp.FullName}»?"))
        {
            try { DatabaseHelper.DeleteEmployee(emp.Id); Load(); }
            catch (Exception ex) { Err(ex); }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private static void NoSel() => MessageBox.Show("Выберите сотрудника.", "Нет выбора",
        MessageBoxButton.OK, MessageBoxImage.Information);
    private static void Err(Exception ex) => MessageBox.Show(ex.Message, "Ошибка",
        MessageBoxButton.OK, MessageBoxImage.Error);
    private static bool Confirm(string msg) =>
        MessageBox.Show(msg, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        == MessageBoxResult.Yes;
}

// ── Диалог редактирования сотрудника ─────────────────────────────────────────
public class EmployeeEditDialog : Window
{
    public Employee? Result { get; private set; }

    private readonly TextBox _tbName     = new();
    private readonly TextBox _tbPhone    = new();
    private readonly TextBox _tbPosition = new();

    public EmployeeEditDialog(Employee? existing)
    {
        Title  = existing == null ? "Новый сотрудник" : "Редактировать сотрудника";
        Width  = 380; Height = 310;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;

        var inputStyle  = Application.Current.Resources["InputStyle"]  as Style;
        var labelStyle  = Application.Current.Resources["FormLabel"]    as Style;
        var accentBtn   = Application.Current.Resources["AccentButton"] as Style;
        var neutralBtn  = Application.Current.Resources["NeutralButton"] as Style;

        _tbName.Style     = inputStyle;  _tbName.Margin     = new Thickness(0, 0, 0, 10);
        _tbPhone.Style    = inputStyle;  _tbPhone.Margin    = new Thickness(0, 0, 0, 10);
        _tbPosition.Style = inputStyle;  _tbPosition.Margin = new Thickness(0, 0, 0, 18);

        if (existing != null)
        {
            _tbName.Text     = existing.FullName;
            _tbPhone.Text    = existing.Phone;
            _tbPosition.Text = existing.Position;
        }

        var lbName     = new TextBlock { Text = "ФИО *:",       Style = labelStyle };
        var lbPhone    = new TextBlock { Text = "Телефон:",      Style = labelStyle };
        var lbPosition = new TextBlock { Text = "Должность:",    Style = labelStyle };

        var btnOk = new Button { Content = "Сохранить", Style = accentBtn,
            Margin = new Thickness(0, 0, 8, 0) };
        var btnCancel = new Button { Content = "Отмена", Style = neutralBtn };

        btnOk.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_tbName.Text))
            { MessageBox.Show("Введите ФИО."); return; }
            Result = new Employee
            {
                Id       = existing?.Id ?? 0,
                FullName = _tbName.Text.Trim(),
                Phone    = _tbPhone.Text.Trim(),
                Position = _tbPosition.Text.Trim()
            };
            DialogResult = true;
        };
        btnCancel.Click += (_, _) => DialogResult = false;

        var btnPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        btnPanel.Children.Add(btnOk);
        btnPanel.Children.Add(btnCancel);

        var sp = new StackPanel { Margin = new Thickness(24) };
        sp.Children.Add(lbName);     sp.Children.Add(_tbName);
        sp.Children.Add(lbPhone);    sp.Children.Add(_tbPhone);
        sp.Children.Add(lbPosition); sp.Children.Add(_tbPosition);
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
