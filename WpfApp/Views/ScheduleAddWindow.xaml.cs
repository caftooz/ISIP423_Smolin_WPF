using System;
using System.Linq;
using System.Windows;

namespace WpfApp.Views
{
    public partial class ScheduleAddWindow : Window
    {
        private MasterServices _ms;
        public ScheduleAddWindow(MasterServices ms)
        {
            InitializeComponent();
            _ms = ms;
            DayCombo.ItemsSource = Core.Context.DaysOfWeek.ToList();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DayCombo.SelectedItem == null) { MessageBox.Show("Выберите день"); return; }
            var parts = TimeBox.Text?.Split(':');
            if (parts == null || parts.Length != 2
                || !int.TryParse(parts[0], out var h) || !int.TryParse(parts[1], out var m)
                || h < 0 || h > 23 || m < 0 || m > 59)
            {
                MessageBox.Show("Введите время в формате чч:мм"); return;
            }
            var schedule = new AppointmentSchedules
            {
                MasterServiceId = _ms.Id,
                DayOfWeekId = ((DaysOfWeek)DayCombo.SelectedItem).Id,
                Time = new TimeSpan(h, m, 0)
            };
            Core.Context.AppointmentSchedules.Add(schedule);
            Core.Context.SaveChanges();
            DialogResult = true;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
