using System.Windows;
using ServiceDesk.Data;
using ServiceDesk.Models;
using System.Windows.Controls;

namespace ServiceDesk.Views.Directories;

public partial class PrioritiesWindow : Window
{
    public PrioritiesWindow() { InitializeComponent(); Load(); }

    private void Load() => Grid.ItemsSource = DatabaseHelper.GetPriorities();

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new PriorityEditDialog(null);
        if (dlg.ShowDialog() == true) { try { DatabaseHelper.SavePriority(dlg.Result!); Load(); } catch (Exception ex) { Err(ex); } }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not Priority p) { NoSel(); return; }
        var dlg = new PriorityEditDialog(p);
        if (dlg.ShowDialog() == true) { try { DatabaseHelper.SavePriority(dlg.Result!); Load(); } catch (Exception ex) { Err(ex); } }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not Priority p) { NoSel(); return; }
        if (Confirm($"Удалить приоритет «{p.Name}»?"))
        {
            try { DatabaseHelper.DeletePriority(p.Id); Load(); }
            catch (Exception ex) { Err(ex); }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private static void NoSel() => MessageBox.Show("Выберите запись.", "Нет выбора",
        MessageBoxButton.OK, MessageBoxImage.Information);
    private static void Err(Exception ex) => MessageBox.Show(ex.Message, "Ошибка",
        MessageBoxButton.OK, MessageBoxImage.Error);
    private static bool Confirm(string msg) =>
        MessageBox.Show(msg, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        == MessageBoxResult.Yes;
}

// ── Диалог редактирования приоритета ─────────────────────────────────────────
public class PriorityEditDialog : Window
{
    public Priority? Result { get; private set; }
    private readonly System.Windows.Controls.TextBox _tbName  = new() { Margin = new Thickness(0, 0, 0, 10) };
    private readonly System.Windows.Controls.TextBox _tbLevel = new() { Margin = new Thickness(0, 0, 0, 16) };

    public PriorityEditDialog(Priority? existing)
    {
        Title  = existing == null ? "Новый приоритет" : "Редактировать приоритет";
        Width  = 340; Height = 260;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;
        Background = System.Windows.Application.Current.Resources["LightBgBrush"] as System.Windows.Media.Brush;

        _tbName.Style  = System.Windows.Application.Current.Resources["InputStyle"] as System.Windows.Style;
        _tbLevel.Style = System.Windows.Application.Current.Resources["InputStyle"] as System.Windows.Style;

        if (existing != null)
        {
            _tbName.Text  = existing.Name;
            _tbLevel.Text = existing.Level.ToString();
        }
        else
        {
            _tbLevel.Text = "1";
        }

        var lbName  = new System.Windows.Controls.TextBlock { Text = "Название:", Style = System.Windows.Application.Current.Resources["FormLabel"] as System.Windows.Style };
        var lbLevel = new System.Windows.Controls.TextBlock { Text = "Уровень (1–4):", Style = System.Windows.Application.Current.Resources["FormLabel"] as System.Windows.Style };

        var btnOk = new System.Windows.Controls.Button { Content = "Сохранить",
            Style = System.Windows.Application.Current.Resources["AccentButton"] as System.Windows.Style,
            Margin = new Thickness(0, 0, 8, 0) };
        var btnCancel = new System.Windows.Controls.Button { Content = "Отмена",
            Style = System.Windows.Application.Current.Resources["NeutralButton"] as System.Windows.Style };

        btnOk.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_tbName.Text)) { MessageBox.Show("Введите название."); return; }
            if (!int.TryParse(_tbLevel.Text, out int lv) || lv < 1 || lv > 10)
            { MessageBox.Show("Уровень должен быть числом."); return; }
            Result = new Priority { Id = existing?.Id ?? 0, Name = _tbName.Text.Trim(), Level = lv };
            DialogResult = true;
        };
        btnCancel.Click += (_, _) => DialogResult = false;

        var btnPanel = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal, HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
        btnPanel.Children.Add(btnOk);
        btnPanel.Children.Add(btnCancel);

        var sp = new System.Windows.Controls.StackPanel { Margin = new Thickness(24) };
        sp.Children.Add(lbName); sp.Children.Add(_tbName);
        sp.Children.Add(lbLevel); sp.Children.Add(_tbLevel);
        sp.Children.Add(btnPanel);

        var border = new Border
        {
            Background = System.Windows.Media.Brushes.White,
            CornerRadius = new CornerRadius(8),
            Margin = new Thickness(16),
            Child = sp
        };
        Content = border;
    }
}
