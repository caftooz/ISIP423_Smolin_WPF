using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class ScheduleAddViewModel : INotifyPropertyChanged
    {
        private MasterServices _ms;
        public event Action<bool> RequestClose;

        public string ServiceName { get; private set; }
        public List<DaysOfWeek> Days { get; } = Core.Context.DaysOfWeek.ToList();

        private DaysOfWeek _selectedDay;
        public DaysOfWeek SelectedDay { get => _selectedDay; set { _selectedDay = value; OnPropertyChanged(); } }
        private string _timeText;
        public string TimeText { get => _timeText; set { _timeText = value; OnPropertyChanged(); } }
        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ScheduleAddViewModel()
        {
            SaveCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (SelectedDay == null) { ErrorMessage = "Выберите день"; return; }
                var parts = TimeText?.Split(':');
                if (parts == null || parts.Length != 2 || !int.TryParse(parts[0], out var h) || !int.TryParse(parts[1], out var m)
                    || h < 0 || h > 23 || m < 0 || m > 59) { ErrorMessage = "Формат: чч:мм"; return; }
                Core.Context.AppointmentSchedules.Add(new AppointmentSchedules
                {
                    MasterServiceId = _ms.Id,
                    DayOfWeekId = SelectedDay.Id,
                    Time = new TimeSpan(h, m, 0)
                });
                Core.Context.SaveChanges();
                RequestClose?.Invoke(true);
            });
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public void Init(MasterServices ms) { _ms = ms; ServiceName = ms.Services?.Name ?? ""; OnPropertyChanged(nameof(ServiceName)); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
