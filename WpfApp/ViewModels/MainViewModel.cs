using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public bool IsLoggedIn => SessionManager.IsLoggedIn;
        public bool IsClientOrNoLoggedIn => !IsLoggedIn || SessionManager.CurrentUser.UserRoleId == 1; 

        public ICommand GoToLoginCommand { get; }
        public ICommand GoToProductsCommand { get; }
        public ICommand GoToServicesCommand { get; }
        public ICommand GoToProfileCommand { get; }
        public ICommand GoToCartCommand { get; }

        public MainViewModel()
        {
            GoToLoginCommand = new RelayCommand(_ =>
            {
                MainWindow.NavigateTo(new LoginPage());
            });
            GoToServicesCommand = new RelayCommand(_ =>
            {
                MainWindow.NavigateTo(new ServicesPage());
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
