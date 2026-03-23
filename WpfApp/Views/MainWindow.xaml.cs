using System.Windows;
using System.Windows.Controls;
using ServiceDesk.Data;
using ServiceDesk.Models;
using ServiceDesk.Views.Directories;

namespace ServiceDesk.Views;

public partial class MainWindow : Window
{
    private List<Request> _allRequests = new();

    public MainWindow()
    {
        InitializeComponent();
        SetupForRole();
        LoadData();
    }

    // ── Роль ─────────────────────────────────────────────────────
    private void SetupForRole()
    {
        var user = App.CurrentUser!;
        TxtUserInfo.Text = $"{user.EmployeeName}\n{user.RoleDisplay}";

        switch (user.Role)
        {
            case "admin":
                // Полный доступ: заявки + все справочники + пользователи
                BtnAdd.Visibility       = Visibility.Visible;
                BtnEdit.Visibility      = Visibility.Visible;
                BtnDelete.Visibility    = Visibility.Visible;
                BtnUsers.Visibility     = Visibility.Visible;
                BtnDirEquip.Visibility  = Visibility.Visible;
                BtnDirStatus.Visibility = Visibility.Visible;
                BtnDirPriority.Visibility  = Visibility.Visible;
                BtnDirEmployees.Visibility = Visibility.Visible;
                CmbExecutorFilter.Visibility = Visibility.Visible;
                LoadExecutorFilter();
                break;

            case "manager":
                // Создание/редактирование заявок + фильтр
                BtnAdd.Visibility    = Visibility.Visible;
                BtnEdit.Visibility   = Visibility.Visible;
                BtnDelete.Visibility = Visibility.Visible;
                CmbExecutorFilter.Visibility = Visibility.Visible;
                LoadExecutorFilter();
                break;

            case "executor":
                // Только свои заявки + смена статуса
                BtnChangeStatus.Visibility = Visibility.Visible;
                break;
        }
    }

    private void LoadExecutorFilter()
    {
        var emps = DatabaseHelper.GetEmployees();
        CmbExecutorFilter.Items.Clear();
        CmbExecutorFilter.Items.Add(new ComboBoxItem { Content = "Все исполнители", Tag = 0 });
        foreach (var e in emps)
            CmbExecutorFilter.Items.Add(new ComboBoxItem { Content = e.FullName, Tag = e.Id });
        CmbExecutorFilter.SelectedIndex = 0;
    }

    // ── Загрузка данных ──────────────────────────────────────────
    private void LoadData()
    {
        try
        {
            int? executorFilter = null;

            // Исполнитель видит только свои заявки
            if (App.CurrentUser!.Role == "executor")
                executorFilter = App.CurrentUser.EmployeeId;
            else if (CmbExecutorFilter.SelectedItem is ComboBoxItem { Tag: int id } && id > 0)
                executorFilter = id;

            _allRequests    = DatabaseHelper.GetRequests(executorFilter);
            ApplySearch();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplySearch()
    {
        var q = TxtSearch.Text.Trim().ToLower();
        var filtered = string.IsNullOrEmpty(q)
            ? _allRequests
            : _allRequests.Where(r =>
                r.Number.ToLower().Contains(q) ||
                r.ClientName.ToLower().Contains(q)).ToList();

        GridRequests.ItemsSource = filtered;
    }

    // ── Выбранная заявка ─────────────────────────────────────────
    private Request? SelectedRequest => GridRequests.SelectedItem as Request;

    // ── Обработчики панели инструментов ──────────────────────────
    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var win = new RequestEditWindow(null);
        if (win.ShowDialog() == true) LoadData();
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedRequest == null) { NoSelectionMsg(); return; }
        var win = new RequestEditWindow(SelectedRequest.Id);
        if (win.ShowDialog() == true) LoadData();
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedRequest == null) { NoSelectionMsg(); return; }
        var r = MessageBox.Show(
            $"Удалить заявку {SelectedRequest.Number}?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (r == MessageBoxResult.Yes)
        {
            DatabaseHelper.DeleteRequest(SelectedRequest.Id);
            LoadData();
        }
    }

    private void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedRequest == null) { NoSelectionMsg(); return; }
        var win = new ChangeStatusWindow(SelectedRequest);
        if (win.ShowDialog() == true) LoadData();
    }

    private void BtnDetails_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedRequest == null) { NoSelectionMsg(); return; }
        new RequestDetailWindow(SelectedRequest).ShowDialog();
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadData();

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplySearch();

    private void CmbExecutorFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadData();

    private void GridRequests_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (SelectedRequest != null)
            new RequestDetailWindow(SelectedRequest).ShowDialog();
    }

    // ── Боковое меню ─────────────────────────────────────────────
    private void BtnRequests_Click(object sender, RoutedEventArgs e)
    {
        TxtPageTitle.Text = "Заявки";
        LoadData();
    }

    private void BtnUsers_Click(object sender, RoutedEventArgs e)
    {
        new UsersWindow().ShowDialog();
    }

    private void BtnDirEquip_Click(object sender, RoutedEventArgs e)
        => new EquipmentTypesWindow().ShowDialog();

    private void BtnDirStatus_Click(object sender, RoutedEventArgs e)
        => new StatusesWindow().ShowDialog();

    private void BtnDirPriority_Click(object sender, RoutedEventArgs e)
        => new PrioritiesWindow().ShowDialog();

    private void BtnDirEmployees_Click(object sender, RoutedEventArgs e)
        => new EmployeesWindow().ShowDialog();

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        App.CurrentUser = null;
        new LoginWindow().Show();
        Close();
    }

    private static void NoSelectionMsg() =>
        MessageBox.Show("Выберите заявку в таблице.", "Нет выбора",
            MessageBoxButton.OK, MessageBoxImage.Information);
}
