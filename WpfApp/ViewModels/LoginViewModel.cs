using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class LoginViewModel : INotifyPropertyChanged
    {
        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _patronymic;
        public string Patronymic
        {
            get => _patronymic;
            set { _patronymic = value; OnPropertyChanged(); }
        }

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        public ICommand ContinueCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand BackCommand { get; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private bool _showPhoneStep = true;
        private bool _showLoginStep;
        private bool _showRegisterStep;

        public bool ShowPhoneStep    { get => _showPhoneStep;    set { _showPhoneStep = value;    OnPropertyChanged(); }  }
        public bool ShowLoginStep    { get => _showLoginStep;    set { _showLoginStep = value;    OnPropertyChanged(); }  }
        public bool ShowRegisterStep { get => _showRegisterStep; set { _showRegisterStep = value; OnPropertyChanged(); }  }

        private Users _foundUser;

        public LoginViewModel()
        {
            ContinueCommand = new RelayCommand(_ =>
            {
                if (String.IsNullOrEmpty(PhoneNumber))
                {
                    ErrorMessage = null;

                    if (string.IsNullOrEmpty(PhoneNumber))
                    {
                        ErrorMessage = "Введите номер телефона полностью";
                        return;
                    }
                }
                _foundUser = Core.Context.Users.FirstOrDefault(u => u.PhoneNumber == PhoneNumber);
                if (_foundUser != null)
                {
                    ShowPhoneStep = false;
                    ShowLoginStep = true;
                }
                else
                {
                    ShowPhoneStep = false;
                    ShowRegisterStep = true;
                }
            });

            LoginCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;

                if (String.IsNullOrEmpty(Password))
                {
                    ErrorMessage = "Введите пароль";
                    return;
                }
                if (_foundUser.Password != Password)
                {
                    ErrorMessage = "Пароль не совпадает";
                    return;
                }

                SessionManager.Login(_foundUser);
                NavigateTo(_foundUser.UserRoleId);
            });

            RegisterCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (String.IsNullOrEmpty(FirstName) || String.IsNullOrEmpty(LastName) ||String.IsNullOrEmpty(Patronymic) || String.IsNullOrEmpty(NewPassword))
                {
                    ErrorMessage = "Заполните все поля";
                    return;
                }
                var newUser = new Users
                {
                    PhoneNumber = PhoneNumber,
                    FirstName = FirstName,
                    LastName = LastName,
                    Patronymic = Patronymic,
                    Password = NewPassword,
                    UserRoleId = 1
                };
                Core.Context.Users.Add(newUser);
                Core.Context.SaveChanges();
                SessionManager.Login(newUser);
                NavigateTo(newUser.UserRoleId);
            });

            BackCommand = new RelayCommand(_ =>
            {
                if (ShowPhoneStep)
                {
                    MainWindow.GoBack();
                    return;
                }

                Password = null;
                FirstName = null;
                LastName = null;
                Patronymic = null;
                NewPassword = null;
                ErrorMessage = null;
                _foundUser = null;

                ShowPhoneStep = true;
                ShowLoginStep = false;
                ShowRegisterStep = false;
            });
        }

        private void NavigateTo(int userRoleId)
        {
            Page nextPage;
            switch (userRoleId)
            {
                case 1:
                    MainWindow.GoBack();
                    return;
                case 2:
                    nextPage = new ManagerPage();
                    break;
                case 3:
                    nextPage = new ManagerPage();
                    break;
                case 4:
                    nextPage = new ManagerPage();
                    break;
                default:
                    nextPage = default;
                    break;
            }

            MainWindow.NavigateTo(nextPage);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
