using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class MasterAppointmentDetailViewModel : INotifyPropertyChanged
    {
        public Users CurrentUser => SessionManager.CurrentUser;

        private Appointments _appointment;
        public Appointments Appointment
        {
            get => _appointment;
            set { _appointment = value; OnPropertyChanged(); }
        }

        public ICommand CompleteCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand LogoutCommand { get; }

        public MasterAppointmentDetailViewModel()
        {
            CompleteCommand = new RelayCommand(_ =>
            {
                if (Appointment == null) return;
                var result = MessageBox.Show("Завершить запись?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Appointment.IsCompleted = true;
                    Core.Context.SaveChanges();
                    MessageBox.Show("Запись завершена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.NavigateTo(new MasterPage());
                }
            });
            BackCommand = new RelayCommand(_ => MainWindow.GoBack());
            LogoutCommand = new RelayCommand(_ => { SessionManager.Logout(); MainWindow.NavigateTo(new ServicesPage()); });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
