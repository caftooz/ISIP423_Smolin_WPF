using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        public bool IsLoggedIn => SessionManager.IsLoggedIn;
        public bool IsClientOrNoLoggedIn => !IsLoggedIn || SessionManager.CurrentUser.UserRoleId == 1; 

        public ICommand GoToLoginCommand { get; }
        public ICommand GoToProductsCommand { get; }
        public ICommand GoToServicesCommand { get; }
        public ICommand GoToProfileCommand { get; }
        public ICommand GoToCartCommand { get; }

        private void UpdateLoggedInState()
        {
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsClientOrNoLoggedIn));
        }

        public MainViewModel()
        {
            SessionManager.OnLogin += UpdateLoggedInState;

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

        public void Dispose()
        {
            SessionManager.OnLogin -= UpdateLoggedInState;
        }
    }
}
