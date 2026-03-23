using System.Windows;
using ServiceDesk.Data;
using ServiceDesk.Models;

namespace ServiceDesk.Views.Directories;

public partial class StatusesWindow : Window
{
    public StatusesWindow() { InitializeComponent(); Load(); }

    private void Load() => Grid.ItemsSource = DatabaseHelper.GetStatuses();

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var name = InputDialog.Show("Новый статус", "Название:", "");
        if (name == null) return;
        try { DatabaseHelper.SaveStatus(new RequestStatus { Name = name }); Load(); }
        catch (Exception ex) { Err(ex); }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not RequestStatus s) { NoSel(); return; }
        var name = InputDialog.Show("Редактировать статус", "Название:", s.Name);
        if (name == null) return;
        try { s.Name = name; DatabaseHelper.SaveStatus(s); Load(); }
        catch (Exception ex) { Err(ex); }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not RequestStatus s) { NoSel(); return; }
        if (Confirm($"Удалить статус «{s.Name}»?"))
        {
            try { DatabaseHelper.DeleteStatus(s.Id); Load(); }
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
