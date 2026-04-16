using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp.Views
{
    public partial class NewAppointmentWindow : Window
    {
        private List<ClientDisplay> _allClients;

        public NewAppointmentWindow()
        {
            InitializeComponent();
            _allClients = Core.Context.Users.Where(u => u.UserRoleId == 1).ToList()
                .Select(u => new ClientDisplay { User = u, DisplayName = $"{u.LastName} {u.FirstName} - {u.PhoneNumber}" }).ToList();
            ClientList.ItemsSource = _allClients;
            ServiceCombo.ItemsSource = Core.Context.Services.ToList();
            PayCombo.ItemsSource = Core.Context.PaymentMethods.ToList();
            DatePick.SelectedDate = DateTime.Today;
        }

        private void ClientSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var s = ClientSearch.Text?.ToLower() ?? "";
            ClientList.ItemsSource = string.IsNullOrEmpty(s) ? _allClients
                : _allClients.Where(c => c.DisplayName.ToLower().Contains(s)).ToList();
        }

        private void ServiceCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            var svc = ServiceCombo.SelectedItem as Services;
            if (svc == null) { MasterCombo.ItemsSource = null; return; }
            MasterCombo.ItemsSource = Core.Context.MasterServices.Where(ms => ms.ServiceId == svc.Id).ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var cd = ClientList.SelectedItem as ClientDisplay;
            if (cd == null) { MessageBox.Show("Выберите клиента"); return; }
            var svc = ServiceCombo.SelectedItem as Services;
            if (svc == null) { MessageBox.Show("Выберите услугу"); return; }
            var ms = MasterCombo.SelectedItem as MasterServices;
            if (ms == null) { MessageBox.Show("Выберите мастера"); return; }
            if (DatePick.SelectedDate == null) { MessageBox.Show("Выберите дату"); return; }
            var parts = TimeBox.Text?.Split(':');
            if (parts == null || parts.Length != 2
                || !int.TryParse(parts[0], out var h) || !int.TryParse(parts[1], out var m))
            { MessageBox.Show("Введите время в формате чч:мм"); return; }
            if (PayCombo.SelectedItem == null) { MessageBox.Show("Выберите способ оплаты"); return; }

            var dt = DatePick.SelectedDate.Value.Date.AddHours(h).AddMinutes(m);
            var appointment = new Appointments
            {
                ServiceId = svc.Id,
                UserClientId = cd.User.Id,
                UserMasterId = ms.UserMasterId,
                AppointmentDateTime = dt,
                CreatedDateTime = DateTime.Now,
                PaymentMethodId = ((PaymentMethods)PayCombo.SelectedItem).Id,
                Comment = CommentBox.Text,
                IsCompleted = false
            };
            Core.Context.Appointments.Add(appointment);
            Core.Context.SaveChanges();
            MessageBox.Show("Запись создана");
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }

    internal class ClientDisplay
    {
        public Users User { get; set; }
        public string DisplayName { get; set; }
    }
}
