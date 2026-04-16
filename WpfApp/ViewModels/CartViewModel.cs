using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CartItemViewModel> Items { get; } = new ObservableCollection<CartItemViewModel>();

        public CartViewModel()
        {
            var items = Core.Context.CartItems
                .Where(c => c.UserClientId == SessionManager.CurrentUser.Id)
                .ToList();

            foreach (var ci in items)
                Items.Add(new CartItemViewModel(ci, this));

            BackCommand = new RelayCommand(_ => MainWindow.GoBack());
            OrderCommand = new RelayCommand(_ => PlaceOrder());
            RecalculateTotals();
        }

        public int ItemCount => Items.Sum(i => i.Quantity);
        public decimal TotalPrice => Items.Sum(i => i.Product.Price * i.Quantity);
        public decimal FinalPrice => Items.Sum(i => i.Product.FinalPrice * i.Quantity);
        public decimal DiscountAmount => TotalPrice - FinalPrice;

        public ICommand OrderCommand { get; }
        public ICommand BackCommand { get; }

        public void RecalculateTotals()
        {
            OnPropertyChanged(nameof(ItemCount));
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(FinalPrice));
            OnPropertyChanged(nameof(DiscountAmount));
        }

        public void RemoveItem(CartItemViewModel item)
        {
            Items.Remove(item);
            RecalculateTotals();
        }

        private void PlaceOrder()
        {
            if (Items.Count == 0)
            {
                MessageBox.Show("Корзина пуста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var w = new OrderWindow(Items.ToList());
            w.Owner = Application.Current.MainWindow;
            var result = w.ShowDialog();

            if (result == true)
            {
                Items.Clear();
                RecalculateTotals();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
