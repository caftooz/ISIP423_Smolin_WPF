using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class UserEditViewModel : INotifyPropertyChanged
    {
        private Users _user;
        private bool _isNew;

        public event Action<bool> RequestClose;

        private string _fullName;
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }

        private string _phone;
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private bool _isFrozen;
        public bool IsFrozen { get => _isFrozen; set { _isFrozen = value; OnPropertyChanged(); } }

        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public List<UserRoles> Roles { get; private set; }

        private UserRoles _selectedRole;
        public UserRoles SelectedRole { get => _selectedRole; set { _selectedRole = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UserEditViewModel()
        {
            Roles = Core.Context.UserRoles.ToList();

            SaveCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (string.IsNullOrWhiteSpace(FullName)) { ErrorMessage = "Введите ФИО"; return; }
                if (string.IsNullOrWhiteSpace(Phone)) { ErrorMessage = "Введите телефон"; return; }
                if (SelectedRole == null) { ErrorMessage = "Выберите роль"; return; }

                var parts = FullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                _user.LastName = parts.Length > 0 ? parts[0] : "";
                _user.FirstName = parts.Length > 1 ? parts[1] : "";
                _user.Patronymic = parts.Length > 2 ? parts[2] : "";
                _user.PhoneNumber = Phone.Trim();
                if (!string.IsNullOrEmpty(Password)) _user.Password = Password;
                _user.UserRoleId = SelectedRole.Id;
                _user.IsFrozen = IsFrozen;

                if (_isNew) Core.Context.Users.Add(_user);
                Core.Context.SaveChanges();
                RequestClose?.Invoke(true);
            });

            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public void Init(Users user)
        {
            _isNew = user == null;
            _user = user ?? new Users();
            if (!_isNew)
            {
                FullName = $"{_user.LastName} {_user.FirstName} {_user.Patronymic}".Trim();
                Phone = _user.PhoneNumber;
                IsFrozen = _user.IsFrozen;
                SelectedRole = Roles.FirstOrDefault(r => r.Id == _user.UserRoleId);
            }
            else
            {
                SelectedRole = Roles.FirstOrDefault();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
