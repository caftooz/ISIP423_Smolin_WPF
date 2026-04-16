using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class AppointmentsViewModel : INotifyPropertyChanged
    {
        public Services CurrentService { get; set; }

        public Users CurrentMaster { get; set; }
        public ObservableCollection<DateTime> MSAppointments { get; } = new ObservableCollection<DateTime>();

        private List<DateTime> _allMSAppointments = new List<DateTime>();

        private DateTime _selectedDT;
        public DateTime SelectedDT
        {
            get => _selectedDT;
            set { _selectedDT = value; OnPropertyChanged(); UpdateAppointments(); }
        }

        private DateTime _selectedAppointment;
        public DateTime SelectedAppointment
        {
            get => _selectedAppointment;
            set { _selectedAppointment = value; OnPropertyChanged(); GoToAppointment(); }
        }

        private void GoToAppointment()
        {
            var dt = SelectedAppointment;

            if (!SessionManager.IsLoggedIn)
            {
                var result = MessageBox.Show("Для записи необходим войти", "Попытка записи", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                
                if (result == MessageBoxResult.OK)
                {
                    MainWindow.NavigateTo(new LoginPage());
                    return;
                }
                else
                {
                    return;
                }
            }

            MasterServices ms = Core.Context.MasterServices.FirstOrDefault(u => u.UserMasterId == CurrentMaster.Id &&
                                                                                           u.ServiceId == CurrentService.Id);
            MainWindow.NavigateTo(new AppointmentDetailPage(ms, dt));
        }

        private void UpdateAppointments()
        {
            var selectedMSAppointments = _allMSAppointments.Where(dt => dt.Day == _selectedDT.Day);

            MSAppointments.Clear();
            foreach (var t in selectedMSAppointments)
            {
                MSAppointments.Add(t);
            }

            OnPropertyChanged(nameof(MSAppointments));
        }

        public void InitializeAppointments()
        {
            MasterServices masterService = Core.Context.MasterServices.FirstOrDefault(u => u.UserMasterId == CurrentMaster.Id &&
                                                                                           u.ServiceId == CurrentService.Id);
            if (masterService == null)
            {
                MessageBox.Show("Данный мастер не проводит данную услугу!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var schedules = Core.Context.AppointmentSchedules.Where(aps => aps.MasterServiceId == masterService.Id);

            if (schedules == null || schedules.Count() <= 0)
            {
                MessageBox.Show("Не найдено расписание проведения данной услуги данным мастером", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var currentDT = DateTime.Now;
            var DT2W = currentDT.AddDays(14);

            while (currentDT < DT2W)
            {
                int dtdow2bddow = currentDT.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)currentDT.DayOfWeek;
                var daySchedule = schedules.Where(sch => sch.DayOfWeekId == dtdow2bddow);

                foreach (var time in daySchedule )
                {
                    var newhm = new DateTime(currentDT.Year, currentDT.Month, currentDT.Day);
                    newhm = newhm.AddHours(time.Time.Hours).AddMinutes(time.Time.Minutes);
                    _allMSAppointments.Add(newhm);
                }

                currentDT = currentDT.AddDays(1);
            }


            MSAppointments.Clear();
            foreach (var t in _allMSAppointments)
            {
                MSAppointments.Add(t);
            }

            OnPropertyChanged(nameof(CurrentMaster));
            OnPropertyChanged(nameof(CurrentService));

            OnPropertyChanged(nameof(MSAppointments));

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
