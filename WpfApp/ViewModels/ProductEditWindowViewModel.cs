using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class ProductEditWindowViewModel : INotifyPropertyChanged
    {
        private Products _product;
        private bool _isNew;
        public event Action<bool> RequestClose;

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
        private string _priceText;
        public string PriceText { get => _priceText; set { _priceText = value; OnPropertyChanged(); } }
        private string _discountText;
        public string DiscountText { get => _discountText; set { _discountText = value; OnPropertyChanged(); } }
        private bool _isNotFrozen;
        public bool IsNotFrozen { get => _isNotFrozen; set { _isNotFrozen = value; OnPropertyChanged(); } }
        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public List<ProductTypes> ProductTypes { get; } = Core.Context.ProductTypes.ToList();
        public List<Manufacturers> Manufacturers { get; } = Core.Context.Manufacturers.ToList();

        private ProductTypes _selectedType;
        public ProductTypes SelectedType { get => _selectedType; set { _selectedType = value; OnPropertyChanged(); } }
        private Manufacturers _selectedManufacturer;
        public Manufacturers SelectedManufacturer { get => _selectedManufacturer; set { _selectedManufacturer = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ProductEditWindowViewModel()
        {
            SaveCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Введите название"; return; }
                if (!decimal.TryParse(PriceText, out var price)) { ErrorMessage = "Некорректная цена"; return; }
                if (!int.TryParse(DiscountText, out var disc)) disc = 0;
                if (SelectedType == null) { ErrorMessage = "Выберите тип"; return; }
                if (SelectedManufacturer == null) { ErrorMessage = "Выберите производителя"; return; }

                _product.Name = Name.Trim();
                _product.Description = Description?.Trim();
                _product.Price = price;
                _product.PercentageDiscount = disc;
                _product.ProductTypeId = SelectedType.Id;
                _product.ManufacturerId = SelectedManufacturer.Id;
                _product.IsFrozen = !IsNotFrozen;
                if (_isNew) Core.Context.Products.Add(_product);
                Core.Context.SaveChanges();
                RequestClose?.Invoke(true);
            });
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public void Init(Products product)
        {
            _isNew = product == null;
            _product = product ?? new Products();
            if (!_isNew)
            {
                Name = _product.Name;
                Description = _product.Description;
                PriceText = _product.Price.ToString("0.00");
                DiscountText = _product.PercentageDiscount.ToString();
                SelectedType = ProductTypes.FirstOrDefault(t => t.Id == _product.ProductTypeId);
                SelectedManufacturer = Manufacturers.FirstOrDefault(m => m.Id == _product.ManufacturerId);
                IsNotFrozen = !_product.IsFrozen;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
