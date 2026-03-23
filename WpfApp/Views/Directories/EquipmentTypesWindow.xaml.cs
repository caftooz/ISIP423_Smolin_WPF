using System.Windows;
using ServiceDesk.Data;
using ServiceDesk.Models;

namespace ServiceDesk.Views.Directories;

public partial class EquipmentTypesWindow : Window
{
    public EquipmentTypesWindow() { InitializeComponent(); Load(); }

    private void Load() => Grid.ItemsSource = DatabaseHelper.GetEquipmentTypes();

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var name = InputDialog.Show("Новый тип техники", "Название:", "");
        if (name == null) return;
        try { DatabaseHelper.SaveEquipmentType(new EquipmentType { Name = name }); Load(); }
        catch (Exception ex) { Err(ex); }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not EquipmentType et) { NoSel(); return; }
        var name = InputDialog.Show("Редактировать тип техники", "Название:", et.Name);
        if (name == null) return;
        try { et.Name = name; DatabaseHelper.SaveEquipmentType(et); Load(); }
        catch (Exception ex) { Err(ex); }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not EquipmentType et) { NoSel(); return; }
        if (Confirm($"Удалить «{et.Name}»?"))
        {
            try { DatabaseHelper.DeleteEquipmentType(et.Id); Load(); }
            catch (Exception ex) { Err(ex); }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private static void NoSel()  => MessageBox.Show("Выберите запись.", "Нет выбора",
        MessageBoxButton.OK, MessageBoxImage.Information);
    private static void Err(Exception ex) => MessageBox.Show(ex.Message, "Ошибка",
        MessageBoxButton.OK, MessageBoxImage.Error);
    private static bool Confirm(string msg) =>
        MessageBox.Show(msg, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        == MessageBoxResult.Yes;
}
