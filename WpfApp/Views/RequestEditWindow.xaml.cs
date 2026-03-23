using System.Windows;
using ServiceDesk.Data;
using ServiceDesk.Models;

namespace ServiceDesk.Views;

public partial class RequestEditWindow : Window
{
    private readonly int? _requestId;
    private Request _request = new();

    public RequestEditWindow(int? requestId)
    {
        InitializeComponent();
        _requestId = requestId;
        LoadCombos();
        if (requestId.HasValue) LoadRequest(requestId.Value);
        else                    InitNew();
    }

    private void LoadCombos()
    {
        // Типы техники
        CmbEquipType.Items.Clear();
        CmbEquipType.Items.Add(new { Id = (int?)null, Name = "— не выбрано —" });
        foreach (var et in DatabaseHelper.GetEquipmentTypes())
            CmbEquipType.Items.Add(new { Id = (int?)et.Id, Name = et.Name });
        CmbEquipType.DisplayMemberPath = "Name";
        CmbEquipType.SelectedValuePath = "Id";
        CmbEquipType.SelectedIndex     = 0;

        // Приоритеты
        CmbPriority.Items.Clear();
        CmbPriority.Items.Add(new { Id = (int?)null, Name = "— не выбрано —" });
        foreach (var p in DatabaseHelper.GetPriorities())
            CmbPriority.Items.Add(new { Id = (int?)p.Id, Name = p.Name });
        CmbPriority.DisplayMemberPath = "Name";
        CmbPriority.SelectedValuePath = "Id";
        CmbPriority.SelectedIndex     = 0;

        // Статусы
        CmbStatus.Items.Clear();
        CmbStatus.Items.Add(new { Id = (int?)null, Name = "— не выбрано —" });
        foreach (var s in DatabaseHelper.GetStatuses())
            CmbStatus.Items.Add(new { Id = (int?)s.Id, Name = s.Name });
        CmbStatus.DisplayMemberPath = "Name";
        CmbStatus.SelectedValuePath = "Id";
        CmbStatus.SelectedIndex     = 0;

        // Исполнители
        CmbExecutor.Items.Clear();
        CmbExecutor.Items.Add(new { Id = (int?)null, Name = "— не назначен —" });
        foreach (var e in DatabaseHelper.GetEmployees())
            CmbExecutor.Items.Add(new { Id = (int?)e.Id, Name = e.FullName });
        CmbExecutor.DisplayMemberPath = "Name";
        CmbExecutor.SelectedValuePath = "Id";
        CmbExecutor.SelectedIndex     = 0;
    }

    private void InitNew()
    {
        TxtTitle.Text  = "Новая заявка";
        var num = DatabaseHelper.GetNextNumber();
        TxtNumber.Text = num;
        _request = new Request
        {
            Number      = num,
            CreatedDate = DateTime.Now,
            CreatedBy   = App.CurrentUser?.Id
        };
        // Статус по умолчанию = "Новая" (первый)
        if (CmbStatus.Items.Count > 1) CmbStatus.SelectedIndex = 1;
        if (CmbPriority.Items.Count > 2) CmbPriority.SelectedIndex = 2; // Средний
    }

    private void LoadRequest(int id)
    {
        _request = DatabaseHelper.GetRequest(id) ?? new Request();
        TxtTitle.Text  = "Редактирование заявки";
        TxtNumber.Text = _request.Number;

        TxtClientName.Text  = _request.ClientName;
        TxtClientPhone.Text = _request.ClientPhone;
        TxtFault.Text       = _request.FaultDescription;
        TxtComment.Text     = _request.RepairComment;
        DpCompletion.SelectedDate = _request.CompletionDate;

        SetComboById(CmbEquipType, _request.EquipmentTypeId);
        SetComboById(CmbPriority,  _request.PriorityId);
        SetComboById(CmbStatus,    _request.StatusId);
        SetComboById(CmbExecutor,  _request.ExecutorId);
    }

    private static void SetComboById(System.Windows.Controls.ComboBox cmb, int? id)
    {
        if (!id.HasValue) { cmb.SelectedIndex = 0; return; }
        for (int i = 0; i < cmb.Items.Count; i++)
        {
            dynamic item = cmb.Items[i]!;
            if (item.Id == id) { cmb.SelectedIndex = i; return; }
        }
    }

    private static int? GetSelectedId(System.Windows.Controls.ComboBox cmb)
    {
        if (cmb.SelectedItem == null) return null;
        dynamic item = cmb.SelectedItem;
        return item.Id;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtClientName.Text))
        {
            MessageBox.Show("Введите ФИО клиента.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(TxtFault.Text))
        {
            MessageBox.Show("Введите описание неисправности.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _request.ClientName       = TxtClientName.Text.Trim();
        _request.ClientPhone      = TxtClientPhone.Text.Trim();
        _request.FaultDescription = TxtFault.Text.Trim();
        _request.RepairComment    = TxtComment.Text.Trim();
        _request.CompletionDate   = DpCompletion.SelectedDate;
        _request.EquipmentTypeId  = GetSelectedId(CmbEquipType);
        _request.PriorityId       = GetSelectedId(CmbPriority);
        _request.StatusId         = GetSelectedId(CmbStatus);
        _request.ExecutorId       = GetSelectedId(CmbExecutor);

        try
        {
            DatabaseHelper.SaveRequest(_request);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения:\n{ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
