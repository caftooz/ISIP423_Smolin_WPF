using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class ProductDetailViewModel : INotifyPropertyChanged
    {
        private ProductModel _product;
        public ProductModel Product
        {
            get => _product;
            set { _product = value; OnPropertyChanged(); }
        }

        public event Action OnGoToLogin;

        private int _quantityInCart;
        public int QuantityInCart
        {
            get => _quantityInCart;
            set { _quantityInCart = value; OnPropertyChanged(); OnQuantityChanged(); }
        }

        private void OnQuantityChanged()
        {
            ShowQuantity = QuantityInCart > 0;
        }

        private bool _showQuantity = false;
        public bool ShowQuantity
        {
            get => _showQuantity;
            set { _showQuantity = value; OnPropertyChanged(); }
        }

        public ICommand ToCartCommand { get; }
        public ICommand IncreaseCountCommand { get; }
        public ICommand DecreaseCountCommand { get; }

        public void Init()
        {
            QuantityInCart = 0;
            if(!SessionManager.IsLoggedIn)
            {
                return;
            }
            if (Core.Context.CartItems.Any(c => c.UserClientId == SessionManager.CurrentUser.Id && c.ProductId == Product.Product.Id))
            {
                var cartItem = Core.Context.CartItems.First(c => c.UserClientId == SessionManager.CurrentUser.Id && c.ProductId == Product.Product.Id);
                QuantityInCart = cartItem.Quantity;
            }
        }

        public ProductDetailViewModel()
        {
            IncreaseCountCommand = new RelayCommand(_ =>
            {
                if (Core.Context.CartItems.Any(c => c.UserClientId == SessionManager.CurrentUser.Id && c.ProductId == Product.Product.Id))
                {
                    var cartItem = Core.Context.CartItems.First(c => c.UserClientId == SessionManager.CurrentUser.Id && c.ProductId == Product.Product.Id);
                    cartItem.Quantity++;
                    Core.Context.SaveChanges();
                    QuantityInCart = cartItem.Quantity;
                    return;
                }
            });
            DecreaseCountCommand = new RelayCommand(_ =>
            {
                if (Core.Context.CartItems.Any(c => c.UserClientId == SessionManager.CurrentUser.Id && c.ProductId == Product.Product.Id))
                {
                    var cartItem = Core.Context.CartItems.First(c => c.UserClientId == SessionManager.CurrentUser.Id && c.ProductId == Product.Product.Id);
                    cartItem.Quantity--;
                    Core.Context.SaveChanges();
                    QuantityInCart = cartItem.Quantity;
                    return;
                }
            });

            ToCartCommand = new RelayCommand(_ =>
            {
                if (!SessionManager.IsLoggedIn)
                {
                    var result = MessageBox.Show("Для добавления в корзину товара необходим войти", "Попытка добавления товара в корзину", MessageBoxButton.OKCancel, MessageBoxImage.Information);

                    if (result == MessageBoxResult.OK)
                    {
                        MainWindow.NavigateTo(new LoginPage());
                        OnGoToLogin?.Invoke();
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                if (Core.Context.CartItems.Any(c => c.UserClientId == SessionManager.CurrentUser.Id && c.ProductId == Product.Product.Id))
                {
                    var cartItem = Core.Context.CartItems.First(c => c.UserClientId == SessionManager.CurrentUser.Id && c.ProductId == Product.Product.Id);
                    cartItem.Quantity++;
                    Core.Context.SaveChanges();
                    MessageBox.Show("Этот товар уже есть в корзине. Кол-во товара в корзине увеличено на 1", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    QuantityInCart = cartItem.Quantity;
                    return;
                }

                Core.Context.CartItems.Add(new CartItems
                {
                    ProductId = Product.Product.Id,
                    UserClientId = SessionManager.CurrentUser.Id,
                    Quantity = 1
                });
                QuantityInCart = 1;
                Core.Context.SaveChanges();


                MessageBox.Show("Товар успешно добавлен в корзину", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
