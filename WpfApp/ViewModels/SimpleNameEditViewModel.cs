using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class SimpleNameEditViewModel : INotifyPropertyChanged
    {
        public event Action<bool> RequestClose;
        private Action<string> _saveAction;

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public SimpleNameEditViewModel()
        {
            SaveCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Введите название"; return; }
                _saveAction?.Invoke(Name.Trim());
                RequestClose?.Invoke(true);
            });
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public void Init(string currentName, Action<string> saveAction)
        {
            Name = currentName;
            _saveAction = saveAction;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
