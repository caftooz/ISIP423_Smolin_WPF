using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class ProfileViewModel : INotifyPropertyChanged
    {
        public Users CurrentUser => SessionManager.CurrentUser;

        public ObservableCollection<Orders> Orders { get; } = new ObservableCollection<Orders>();
        public ObservableCollection<Appointments> Appointments { get; } = new ObservableCollection<Appointments>();

        private bool _showOrders = true;
        public bool ShowOrders
        {
            get => _showOrders;
            set { _showOrders = value; OnPropertyChanged(); }
        }

        private bool _showAppointments;
        public bool ShowAppointments
        {
            get => _showAppointments;
            set { _showAppointments = value; OnPropertyChanged(); }
        }

        public ICommand ShowOrdersCommand { get; }
        public ICommand ShowAppointmentsCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand LogoutCommand { get; }

        public ProfileViewModel()
        {
            if (SessionManager.IsLoggedIn)
            {
                var userId = CurrentUser.Id;
                foreach (var o in Core.Context.Orders.Where(o => o.UserClientId == userId).ToList())
                    Orders.Add(o);
                foreach (var a in Core.Context.Appointments.Where(a => a.UserClientId == userId).ToList())
                    Appointments.Add(a);
            }

            ShowOrdersCommand = new RelayCommand(_ => { ShowOrders = true; ShowAppointments = false; });
            ShowAppointmentsCommand = new RelayCommand(_ => { ShowOrders = false; ShowAppointments = true; });
            BackCommand = new RelayCommand(_ => MainWindow.GoBack());
            LogoutCommand = new RelayCommand(_ =>
            {
                SessionManager.Logout();
                MainWindow.NavigateTo(new ServicesPage());
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
