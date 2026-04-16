using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class OrderWindow : Window
    {
        private List<CartItemViewModel> _items;
        private decimal _total;

        public OrderWindow(List<CartItemViewModel> items)
        {
            InitializeComponent();
            _items = items;
            _total = items.Sum(i => i.Product.FinalPrice * i.Quantity);

            ItemsList.ItemsSource = items.Select(i => $"{i.Product.Name} x{i.Quantity} — {(i.Product.FinalPrice * i.Quantity):0.00} ₽").ToList();
            TotalBlock.Text = $"{_total:0.00} ₽";
            PayCombo.ItemsSource = Core.Context.PaymentMethods.ToList();
        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {
            if (PayCombo.SelectedItem == null) { MessageBox.Show("Выберите способ оплаты"); return; }

            var order = new Orders
            {
                UserClientId = SessionManager.CurrentUser.Id,
                OrderDateTime = DateTime.Now,
                ReceivedDate = DateTime.Now.AddDays(3),
                PaymentMethodId = ((PaymentMethods)PayCombo.SelectedItem).Id,
                TotalCost = _total,
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
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
