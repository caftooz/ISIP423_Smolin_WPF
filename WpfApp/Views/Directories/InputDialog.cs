using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ServiceDesk.Views.Directories;

/// <summary>
/// Универсальный диалог ввода одной строки (используется в справочниках).
/// </summary>
public class InputDialog : Window
{
    private readonly TextBox _tb = new();
    public string? ResultValue { get; private set; }

    private InputDialog(string title, string label, string defaultValue)
    {
        Title  = title;
        Width  = 360; Height = 200;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;

        _tb.Style  = Application.Current.Resources["InputStyle"] as Style;
        _tb.Text   = defaultValue;
        _tb.Margin = new Thickness(0, 0, 0, 16);
        _tb.SelectAll();

        var lbl = new TextBlock
        {
            Text   = label,
            Style  = Application.Current.Resources["FormLabel"] as Style
        };

        var btnOk = new Button
        {
            Content = "OK",
            Style   = Application.Current.Resources["AccentButton"] as Style,
            Margin  = new Thickness(0, 0, 8, 0)
        };
        var btnCancel = new Button
        {
            Content = "Отмена",
            Style   = Application.Current.Resources["NeutralButton"] as Style
        };

        btnOk.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_tb.Text))
            { MessageBox.Show("Поле не может быть пустым.", "Проверка"); return; }
            ResultValue  = _tb.Text.Trim();
            DialogResult = true;
        };
        btnCancel.Click += (_, _) => DialogResult = false;

        // Enter/Escape
        _tb.KeyDown += (_, e) =>
        {
            if (e.Key == System.Windows.Input.Key.Enter)  btnOk.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            if (e.Key == System.Windows.Input.Key.Escape) btnCancel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        };

        var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        btnPanel.Children.Add(btnOk);
        btnPanel.Children.Add(btnCancel);

        var sp = new StackPanel { Margin = new Thickness(24) };
        sp.Children.Add(lbl);
        sp.Children.Add(_tb);
        sp.Children.Add(btnPanel);

        Content = new Border
        {
            Background   = Brushes.White,
            CornerRadius = new CornerRadius(8),
            Margin       = new Thickness(16),
            Child        = sp
        };
        Background = Application.Current.Resources["LightBgBrush"] as Brush;
        Loaded += (_, _) => _tb.Focus();
    }

    /// <summary>
    /// Показывает диалог и возвращает введённое значение или null при отмене.
    /// </summary>
    public static string? Show(string title, string label, string defaultValue = "",
                                Window? owner = null)
    {
        var dlg = new InputDialog(title, label, defaultValue) { Owner = owner ?? Application.Current.MainWindow };
        return dlg.ShowDialog() == true ? dlg.ResultValue : null;
    }
}
