using System;
using System.Linq;
using System.Windows;

namespace WpfApp.Views
{
    public partial class ProductEditWindow : Window
    {
        private Products _product;
        private bool _isNew;

        public ProductEditWindow(Products product)
        {
            InitializeComponent();
            _isNew = product == null;
            _product = product ?? new Products();

            TypeCombo.ItemsSource = Core.Context.ProductTypes.ToList();
            ManufCombo.ItemsSource = Core.Context.Manufacturers.ToList();

            if (!_isNew)
            {
                NameBox.Text = _product.Name;
                DescBox.Text = _product.Description;
                PriceBox.Text = _product.Price.ToString("0.00");
                DiscountBox.Text = _product.PercentageDiscount.ToString();
                TypeCombo.SelectedValue = Core.Context.ProductTypes.FirstOrDefault(t => t.Id == _product.ProductTypeId);
                ManufCombo.SelectedValue = Core.Context.Manufacturers.FirstOrDefault(m => m.Id == _product.ManufacturerId);
                FrozenCheck.IsChecked = _product.IsFrozen;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text)) { MessageBox.Show("Введите название"); return; }
            if (!decimal.TryParse(PriceBox.Text, out var price)) { MessageBox.Show("Некорректная цена"); return; }
            if (!int.TryParse(DiscountBox.Text, out var disc)) disc = 0;
            if (TypeCombo.SelectedItem == null) { MessageBox.Show("Выберите тип"); return; }
            if (ManufCombo.SelectedItem == null) { MessageBox.Show("Выберите производителя"); return; }

            _product.Name = NameBox.Text.Trim();
            _product.Description = DescBox.Text?.Trim();
            _product.Price = price;
            _product.PercentageDiscount = disc;
            _product.ProductTypeId = ((ProductTypes)TypeCombo.SelectedItem).Id;
            _product.ManufacturerId = ((Manufacturers)ManufCombo.SelectedItem).Id;
            _product.IsFrozen = FrozenCheck.IsChecked == true;

            if (_isNew) Core.Context.Products.Add(_product);
            Core.Context.SaveChanges();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
