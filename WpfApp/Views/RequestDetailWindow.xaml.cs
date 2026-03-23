using System.Windows;
using System.Windows.Media;
using ServiceDesk.Models;

namespace ServiceDesk.Views;

public partial class RequestDetailWindow : Window
{
    public RequestDetailWindow(Request r)
    {
        InitializeComponent();
        Populate(r);
    }

    private void Populate(Request r)
    {
        TxtNumber.Text     = r.Number;
        TxtCreated.Text    = $"Создана: {r.CreatedDateStr}";
        TxtClientName.Text = r.ClientName;
        TxtClientPhone.Text = string.IsNullOrWhiteSpace(r.ClientPhone) ? "—" : r.ClientPhone;
        TxtEquip.Text      = string.IsNullOrWhiteSpace(r.EquipmentTypeName) ? "—" : r.EquipmentTypeName;
        TxtExecutor.Text   = string.IsNullOrWhiteSpace(r.ExecutorName) ? "Не назначен" : r.ExecutorName;
        TxtFault.Text      = string.IsNullOrWhiteSpace(r.FaultDescription) ? "—" : r.FaultDescription;
        TxtComment.Text    = string.IsNullOrWhiteSpace(r.RepairComment) ? "Комментарий не добавлен" : r.RepairComment;
        TxtCompletion.Text = r.CompletionDateStr;
        TxtPriority.Text   = r.PriorityName;
        TxtStatus.Text     = r.StatusName;

        // Цвет приоритета
        BadgePriority.Background = r.PriorityLevel switch
        {
            4 => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
            3 => new SolidColorBrush(Color.FromRgb(243, 156, 18)),
            2 => new SolidColorBrush(Color.FromRgb(52, 152, 219)),
            _ => new SolidColorBrush(Color.FromRgb(149, 165, 166))
        };
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
