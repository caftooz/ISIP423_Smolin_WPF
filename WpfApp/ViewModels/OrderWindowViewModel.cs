using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class OrderWindowViewModel : INotifyPropertyChanged
    {
        public event Action<bool> RequestClose;
        private List<CartItemViewModel> _items;

        public ObservableCollection<OrderLineItem> ItemLines { get; } = new ObservableCollection<OrderLineItem>();
        public List<PaymentMethods> PaymentMethods { get; } = Core.Context.PaymentMethods.ToList();

        private DateTime _receivedDate = DateTime.Today.AddDays(3);
        public DateTime MinDate { get; } = DateTime.Today;
        public DateTime MaxDate { get; } = DateTime.Today.AddDays(7);
        public DateTime ReceivedDate { get => _receivedDate; set { _receivedDate = value; OnPropertyChanged(); } }

        private PaymentMethods _selectedPayment;
        public PaymentMethods SelectedPayment { get => _selectedPayment; set { _selectedPayment = value; OnPropertyChanged(); } }

        public decimal TotalPrice { get; private set; }
        public decimal FinalPrice { get; private set; }
        public decimal DiscountAmount { get; private set; }

        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand PlaceOrderCommand { get; }

        public OrderWindowViewModel()
        {
            PlaceOrderCommand = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (SelectedPayment == null) { ErrorMessage = "Выберите способ оплаты"; return; }

                var order = new Orders
                {
                    UserClientId = SessionManager.CurrentUser.Id,
                    OrderDateTime = DateTime.Now,
                    ReceivedDate = ReceivedDate,
                    PaymentMethodId = SelectedPayment.Id,
                    TotalCost = FinalPrice,
                    IsCompleted = false
                };
                Core.Context.Orders.Add(order);
                Core.Context.SaveChanges();

                foreach (var item in _items)
                {
                    Core.Context.OrderProducts.Add(new OrderProducts
                    {
                        OrderId = order.Id,
                        ProductId = item.Product.Product.Id,
                        Quantity = item.Quantity
                    });
                    Core.Context.CartItems.Remove(item.CartItem);
                }
                Core.Context.SaveChanges();
                MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke(true);
            });
        }

        public void Init(List<CartItemViewModel> items)
        {
            _items = items;
            TotalPrice = items.Sum(i => i.Product.Price * i.Quantity);
            FinalPrice = items.Sum(i => i.Product.FinalPrice * i.Quantity);
            DiscountAmount = TotalPrice - FinalPrice;
            foreach (var i in items)
                ItemLines.Add(new OrderLineItem
                {
                    Name = i.Product.Name,
                    PriceLine = $"{i.Product.FinalPrice:N0} ₽ x{i.Quantity} = {(i.Product.FinalPrice * i.Quantity):N0} ₽"
                });
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(FinalPrice));
            OnPropertyChanged(nameof(DiscountAmount));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class OrderLineItem
    {
        public string Name { get; set; }
        public string PriceLine { get; set; }
    }
}
