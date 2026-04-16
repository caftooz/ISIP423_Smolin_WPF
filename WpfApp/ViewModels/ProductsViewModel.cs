using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class ProductsViewModel : INotifyPropertyChanged
    {
        private string _findText;
        public string FindText
        {
            get => _findText;
            set { _findText = value; OnPropertyChanged(); UpdateProducts(); }
        }

        public Dictionary<int , string> SortedType { get; set; } = new Dictionary<int, string> { 
            { 1, "по оценке" },
            { 2, "по названию" }, 
            { 3, "по цене" } };

        private KeyValuePair<int, string> _selectedSortedType;
        public KeyValuePair<int, string> SelectedSortedType
        {
            get => _selectedSortedType;
            set { _selectedSortedType = value; OnPropertyChanged(); UpdateProducts(); }
        }

        private ProductModel _selectedProduct;
        public ProductModel SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
                var product = _selectedProduct;
                
                if (product == null)
                    return;

                var window = new ProductDetailWindow(product);
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _selectedProduct = null;
                    OnPropertyChanged(nameof(SelectedProduct));
                }));
            }
        }

        private List<FilterChoice<Manufacturers>> _manufacturers;
        public List<FilterChoice<Manufacturers>> Manufacturers => _manufacturers;


        private List<FilterChoice<ProductTypes>> _productTypes;
        public List<FilterChoice<ProductTypes>> ProductTypes => _productTypes;

        public ObservableCollection<ProductModel> Products { get; } = new ObservableCollection<ProductModel>();

        public ProductsViewModel()
        {
            _manufacturers = Core.Context.Manufacturers.ToList().Select(m => new FilterChoice<Manufacturers>(m, UpdateProducts)).ToList();
            _productTypes = Core.Context.ProductTypes.ToList().Select(s => new FilterChoice<ProductTypes>(s, UpdateProducts)).ToList();
            UpdateProducts();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void UpdateProducts()
        {
            var products = Core.Context.Products.ToList().AsQueryable();

            if (_productTypes.Any(s => s.IsActive))
            {
                var activeTypes = _productTypes.Where(st => st.IsActive);
                products = products.Where(p => activeTypes.Any(st => p.ProductTypeId == st.Item.Id));
            }
            if (_manufacturers.Any(m => m.IsActive))
            {
                var activeManufacturersIds = _manufacturers.Where(m => m.IsActive).Select(m => m.Item.Id);

                products = products.Where(p => activeManufacturersIds.Any(amId => p.ManufacturerId == amId));
            }
            if (!string.IsNullOrEmpty(FindText))
            {
                products = products.Where(p => p.Name.ToLower().Contains(FindText.ToLower()));
            }

            switch (SelectedSortedType.Key)
            {
                case 1:
                    products = products.OrderByDescending(p => p.Rating);
                    break;
                case 2:
                    products = products.OrderBy(p => p.Name);
                    break;
                case 3:
                    products = products.OrderBy(p => p.Price);
                    break;
            }

            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(new ProductModel(product));
            }
            OnPropertyChanged(nameof(Products));
        }

    }

    public class ProductModel
    {
        public Products Product { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int PercentageDiscount { get; }
        public decimal FinalPrice { get; }
        public double Rating { get; }
        public string ManufacturerName { get; }
        public string Image { get; }
        public string Description { get; }
        public bool ShowDiscount => PercentageDiscount > 0;
        public ProductModel(Products product)
        {
            Product = product;
            Name = product.Name;
            Price = product.Price;
            PercentageDiscount = product.PercentageDiscount;
            FinalPrice = Price * (100 - PercentageDiscount) / 100;
            Rating = product.Rating;
            ManufacturerName = product.Manufacturers.Name;
            Image = product.Image;
            Description = product.Description;
        }
    }
}
