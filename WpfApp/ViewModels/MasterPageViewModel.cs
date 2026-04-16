using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class MasterPageViewModel : INotifyPropertyChanged
    {
        public Users CurrentUser => SessionManager.CurrentUser;

        public ObservableCollection<MasterServiceItem> MasterServiceItems { get; } = new ObservableCollection<MasterServiceItem>();
        public ObservableCollection<Appointments> MasterAppointments { get; } = new ObservableCollection<Appointments>();

        private bool _showServicesTab = true;
        public bool ShowServicesTab { get => _showServicesTab; set { _showServicesTab = value; OnPropertyChanged(); } }

        private bool _showAppointmentsTab;
        public bool ShowAppointmentsTab { get => _showAppointmentsTab; set { _showAppointmentsTab = value; OnPropertyChanged(); } }

        private Appointments _selectedAppointment;
        public Appointments SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                _selectedAppointment = value;
                OnPropertyChanged();
                if (value != null)
                    MainWindow.NavigateTo(new MasterAppointmentDetailPage(value));
            }
        }

        public ICommand ShowServicesTabCommand { get; }
        public ICommand ShowAppointmentsTabCommand { get; }
        public ICommand LogoutCommand { get; }

        public MasterPageViewModel()
        {
            if (SessionManager.IsLoggedIn)
            {
                var uid = CurrentUser.Id;
                var allServices = Core.Context.Services.ToList();
                var masterSvcIds = Core.Context.MasterServices.Where(ms => ms.UserMasterId == uid).Select(ms => ms.ServiceId).ToList();
                foreach (var s in allServices)
                    MasterServiceItems.Add(new MasterServiceItem { ServiceName = s.Name, IsActive = masterSvcIds.Contains(s.Id) });

                foreach (var a in Core.Context.Appointments.Where(a => a.UserMasterId == uid && !a.IsCompleted).ToList()
                    .OrderBy(a => a.AppointmentDateTime))
                    MasterAppointments.Add(a);
            }

            ShowServicesTabCommand = new RelayCommand(_ => { ShowServicesTab = true; ShowAppointmentsTab = false; });
            ShowAppointmentsTabCommand = new RelayCommand(_ => { ShowServicesTab = false; ShowAppointmentsTab = true; });
            LogoutCommand = new RelayCommand(_ => { SessionManager.Logout(); MainWindow.NavigateTo(new ServicesPage()); });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class MasterServiceItem
    {
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
    }
}
