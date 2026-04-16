using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class ServiceEditWindowViewModel : INotifyPropertyChanged
    {
        private Services _item;
        private bool _isNew;
        public event Action<bool> RequestClose;

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        private string _priceText;
        public string PriceText { get => _priceText; set { _priceText = value; OnPropertyChanged(); } }
        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public List<ServiceTypes> ServiceTypes { get; } = Core.Context.ServiceTypes.ToList();
        private ServiceTypes _selectedType;
        public ServiceTypes SelectedType { get => _selectedType; set { _selectedType = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ServiceEditWindowViewModel()
        {
            SaveCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Введите название"; return; }
                if (!decimal.TryParse(PriceText, out var price)) { ErrorMessage = "Некорректная цена"; return; }
                if (SelectedType == null) { ErrorMessage = "Выберите тип"; return; }
                _item.Name = Name.Trim();
                _item.Price = price;
                _item.ServiceTypeId = SelectedType.Id;
                if (_isNew) Core.Context.Services.Add(_item);
                Core.Context.SaveChanges();
                RequestClose?.Invoke(true);
            });
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public void Init(Services item)
        {
            _isNew = item == null;
            _item = item ?? new Services();
            if (!_isNew)
            {
                Name = _item.Name;
                PriceText = _item.Price.ToString("0.00");
                SelectedType = ServiceTypes.FirstOrDefault(t => t.Id == _item.ServiceTypeId);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
