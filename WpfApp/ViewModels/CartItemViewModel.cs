using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        private readonly CartViewModel _parent;
        public CartItems CartItem { get; }
        public ProductModel Product { get; }

        public decimal Price => Product.Price * Quantity;
        public decimal FinalPrice => Product.FinalPrice * Quantity;

        public CartItemViewModel(CartItems cartItem, CartViewModel parent)
        {
            CartItem = cartItem;
            Product = new ProductModel(cartItem.Products);
            _parent = parent;

            IncreaseCommand = new RelayCommand(_ => Increase());
            DecreaseCommand = new RelayCommand(_ => Decrease());
            RemoveCommand = new RelayCommand(_ => Remove());
        }

        public int Quantity
        {
            get => CartItem.Quantity;
            set
            {
                CartItem.Quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(FinalPrice));
                OnPropertyChanged(nameof(LineTotal));
                _parent.RecalculateTotals();
            }
        }

        public decimal LineTotal => Product.FinalPrice * Quantity;

        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        public ICommand RemoveCommand { get; }

        private void Increase()
        {
            Quantity++;
            Core.Context.SaveChanges();
        }

        private void Decrease()
        {
            if (Quantity <= 1) { Remove(); return; }
            Quantity--;
            Core.Context.SaveChanges();
        }

        private void Remove()
        {
            Core.Context.CartItems.Remove(CartItem);
            Core.Context.SaveChanges();
            _parent.RemoveItem(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
