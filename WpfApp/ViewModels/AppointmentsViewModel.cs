using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp.ViewModels
{
    internal class AppointmentsViewModel : INotifyPropertyChanged
    {
        public Services CurrentService { get; set; }

        public Users CurrentMaster { get; set; }

        public ObservableCollection<DateTime> MSAppointments { get; } = new ObservableCollection<DateTime>();

        public void UpdateAppointments()
        {
            MasterServices masterService = Core.Context.MasterServices.FirstOrDefault(u => u.UserMasterId == CurrentMaster.Id &&
                                                                                           u.ServiceId == CurrentService.Id);
            if (masterService == null)
            {
                // ex
                return;
            }

            var schedules = Core.Context.AppointmentSchedules.Where(aps => aps.MasterServiceId == masterService.Id);

            if (schedules == null)
            {
                // ex
                return;
            }

            List<DateTime> tmp = new List<DateTime>();

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
                    tmp.Add(newhm);
                }

                currentDT = currentDT.AddDays(1);
            }


            MSAppointments.Clear();
            foreach (var t in tmp)
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
