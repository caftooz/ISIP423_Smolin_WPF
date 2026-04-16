using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class NewAppointmentViewModel : INotifyPropertyChanged
    {
        public event Action<bool> RequestClose;

        private List<Users> _allClients;
        public ObservableCollection<Users> FilteredClients { get; } = new ObservableCollection<Users>();
        public List<Services> AllServices { get; }
        public ObservableCollection<MasterServices> AvailableMasters { get; } = new ObservableCollection<MasterServices>();
        public List<PaymentMethods> PaymentMethods { get; }

        private string _clientSearch;
        public string ClientSearch { get => _clientSearch; set { _clientSearch = value; OnPropertyChanged(); FilterClients(); } }

        private Users _selectedClient;
        public Users SelectedClient { get => _selectedClient; set { _selectedClient = value; OnPropertyChanged(); } }

        private Services _selectedService;
        public Services SelectedService { get => _selectedService; set { _selectedService = value; OnPropertyChanged(); RefreshMasters(); } }

        private MasterServices _selectedMasterService;
        public MasterServices SelectedMasterService { get => _selectedMasterService; set { _selectedMasterService = value; OnPropertyChanged(); } }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(); } }

        private string _timeText;
        public string TimeText { get => _timeText; set { _timeText = value; OnPropertyChanged(); } }

        private PaymentMethods _selectedPayment;
        public PaymentMethods SelectedPayment { get => _selectedPayment; set { _selectedPayment = value; OnPropertyChanged(); } }

        private string _comment;
        public string Comment { get => _comment; set { _comment = value; OnPropertyChanged(); } }

        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public NewAppointmentViewModel()
        {
            _allClients = Core.Context.Users.Where(u => u.UserRoleId == 1).ToList();
            AllServices = Core.Context.Services.ToList();
            PaymentMethods = Core.Context.PaymentMethods.ToList();
            FilterClients();

            SaveCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (SelectedClient == null) { ErrorMessage = "Выберите клиента"; return; }
                if (SelectedService == null) { ErrorMessage = "Выберите услугу"; return; }
                if (SelectedMasterService == null) { ErrorMessage = "Выберите мастера"; return; }
                if (SelectedPayment == null) { ErrorMessage = "Выберите способ оплаты"; return; }
                var parts = TimeText?.Split(':');
                if (parts == null || parts.Length != 2 || !int.TryParse(parts[0], out var h) || !int.TryParse(parts[1], out var m))
                { ErrorMessage = "Формат времени: чч:мм"; return; }

                Core.Context.Appointments.Add(new Appointments
                {
                    ServiceId = SelectedService.Id,
                    UserClientId = SelectedClient.Id,
                    UserMasterId = SelectedMasterService.UserMasterId,
                    AppointmentDateTime = SelectedDate.Date.AddHours(h).AddMinutes(m),
                    CreatedDateTime = DateTime.Now,
                    PaymentMethodId = SelectedPayment.Id,
                    Comment = Comment,
                    IsCompleted = false
                });
                Core.Context.SaveChanges();
                MessageBox.Show("Запись создана", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke(true);
            });
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        private void FilterClients()
        {
            FilteredClients.Clear();
            var list = string.IsNullOrEmpty(ClientSearch) ? _allClients
                : _allClients.Where(c => $"{c.LastName} {c.FirstName} {c.PhoneNumber}".ToLower().Contains(ClientSearch.ToLower())).ToList();
            foreach (var c in list) FilteredClients.Add(c);
        }

        private void RefreshMasters()
        {
            AvailableMasters.Clear();
            if (SelectedService == null) return;
            foreach (var ms in Core.Context.MasterServices.Where(ms => ms.ServiceId == SelectedService.Id).ToList())
                AvailableMasters.Add(ms);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
