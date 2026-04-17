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
        public ICommand BackCommand { get; }

        public MasterPageViewModel()
        {
            if (SessionManager.IsLoggedIn)
            {
                var uid = CurrentUser.Id;
                var allServices = Core.Context.Services.ToList();
                foreach (var s in allServices)
                {
                    var existing = Core.Context.MasterServices
                        .FirstOrDefault(ms => ms.UserMasterId == uid && ms.ServiceId == s.Id);
                    MasterServiceItems.Add(new MasterServiceItem(uid, s.Id, s.Name, existing != null));
                }

                foreach (var a in Core.Context.Appointments
                    .Where(a => a.UserMasterId == uid && !a.IsCompleted).ToList()
                    .OrderBy(a => a.AppointmentDateTime))
                    MasterAppointments.Add(a);
            }

            ShowServicesTabCommand = new RelayCommand(_ => { ShowServicesTab = true; ShowAppointmentsTab = false; });
            ShowAppointmentsTabCommand = new RelayCommand(_ => { ShowServicesTab = false; ShowAppointmentsTab = true; });
            BackCommand = new RelayCommand(_ => MainWindow.GoBack());
            LogoutCommand = new RelayCommand(_ => { SessionManager.Logout(); MainWindow.NavigateTo(new ServicesPage()); });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class MasterServiceItem : INotifyPropertyChanged
    {
        private readonly int _masterId;
        private readonly int _serviceId;

        public string ServiceName { get; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                OnPropertyChanged();
                SaveToDb();
            }
        }

        public MasterServiceItem(int masterId, int serviceId, string serviceName, bool isActive)
        {
            _masterId = masterId;
            _serviceId = serviceId;
            ServiceName = serviceName;
            _isActive = isActive;
        }

        private void SaveToDb()
        {
            var existing = Core.Context.MasterServices
                .FirstOrDefault(ms => ms.UserMasterId == _masterId && ms.ServiceId == _serviceId);

            if (_isActive && existing == null)
            {
                Core.Context.MasterServices.Add(new MasterServices
                {
                    UserMasterId = _masterId,
                    ServiceId = _serviceId
                });
                Core.Context.SaveChanges();
            }
            else if (!_isActive && existing != null)
            {
                Core.Context.MasterServices.Remove(existing);
                Core.Context.SaveChanges();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
