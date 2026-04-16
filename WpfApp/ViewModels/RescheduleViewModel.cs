using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class RescheduleViewModel : INotifyPropertyChanged
    {
        private Appointments _appointment;
        public event Action<bool> RequestClose;

        private DateTime _selectedDate;
        public DateTime SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(); } }
        private string _timeText;
        public string TimeText { get => _timeText; set { _timeText = value; OnPropertyChanged(); } }
        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public RescheduleViewModel()
        {
            SaveCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                var parts = TimeText?.Split(':');
                if (parts == null || parts.Length != 2 || !int.TryParse(parts[0], out var h) || !int.TryParse(parts[1], out var m))
                { ErrorMessage = "Формат: чч:мм"; return; }
                _appointment.AppointmentDateTime = SelectedDate.Date.AddHours(h).AddMinutes(m);
                Core.Context.SaveChanges();
                RequestClose?.Invoke(true);
            });
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public void Init(Appointments a)
        {
            _appointment = a;
            SelectedDate = a.AppointmentDateTime.Date;
            TimeText = a.AppointmentDateTime.ToString("HH:mm");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
