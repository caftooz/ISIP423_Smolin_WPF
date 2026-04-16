using System;
using System.Windows;

namespace WpfApp.Views
{
    public partial class RescheduleAppointmentWindow : Window
    {
        private Appointments _appointment;
        public RescheduleAppointmentWindow(Appointments appointment)
        {
            InitializeComponent();
            _appointment = appointment;
            DatePick.SelectedDate = appointment.AppointmentDateTime.Date;
            TimeBox.Text = appointment.AppointmentDateTime.ToString("HH:mm");
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DatePick.SelectedDate == null) { MessageBox.Show("Выберите дату"); return; }
            var parts = TimeBox.Text?.Split(':');
            if (parts == null || parts.Length != 2
                || !int.TryParse(parts[0], out var h) || !int.TryParse(parts[1], out var m))
            { MessageBox.Show("Введите время в формате чч:мм"); return; }
            _appointment.AppointmentDateTime = DatePick.SelectedDate.Value.Date.AddHours(h).AddMinutes(m);
            Core.Context.SaveChanges();
            DialogResult = true;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
