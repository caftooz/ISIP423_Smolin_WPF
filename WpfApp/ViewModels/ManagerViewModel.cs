using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class ManagerViewModel : INotifyPropertyChanged
    {
        public Users CurrentUser => SessionManager.CurrentUser;
        private Frame _frame;

        public ICommand NavCommand { get; }
        public ICommand LogoutCommand { get; }

        public void SetFrame(Frame frame) { _frame = frame; }

        public ManagerViewModel()
        {
            NavCommand = new RelayCommand(p =>
            {
                if (_frame == null) return;
                Page page = null;
                switch ((string)p)
                {
                    case "appointments": page = new ManagerAppointmentsPage(); break;
                    case "orders": page = new ManagerOrdersPage(); break;
                    case "products": page = new ManagerProductsPage(); break;
                    case "manufacturers": page = new ManagerManufacturersPage(); break;
                    case "productTypes": page = new ManagerProductTypesPage(); break;
                    case "services": page = new ManagerServicesPage(); break;
                    case "serviceTypes": page = new ManagerServiceTypesPage(); break;
                    case "schedules": page = new ManagerSchedulesPage(); break;
                }
                if (page != null) _frame.Navigate(page);
            });

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
