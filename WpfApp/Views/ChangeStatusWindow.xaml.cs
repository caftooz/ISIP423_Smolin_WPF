using System.Windows;
using ServiceDesk.Data;
using ServiceDesk.Models;

namespace ServiceDesk.Views;

public partial class ChangeStatusWindow : Window
{
    private readonly Request _request;

    public ChangeStatusWindow(Request request)
    {
        InitializeComponent();
        _request = request;
        TxtRequestNum.Text = $"Заявка {request.Number}  —  {request.ClientName}";

        var statuses = DatabaseHelper.GetStatuses();
        CmbStatus.ItemsSource       = statuses;
        CmbStatus.DisplayMemberPath = "Name";
        CmbStatus.SelectedValuePath = "Id";
        var cur = statuses.FirstOrDefault(s => s.Id == request.StatusId);
        if (cur != null) CmbStatus.SelectedItem = cur;
        else             CmbStatus.SelectedIndex = 0;
    }

    private void BtnApply_Click(object sender, RoutedEventArgs e)
    {
        if (CmbStatus.SelectedItem is not RequestStatus s) return;
        try
        {
            DatabaseHelper.UpdateRequestStatus(_request.Id, s.Id);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
