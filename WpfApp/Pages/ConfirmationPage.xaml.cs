using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Confirmation.xaml
    /// </summary>
    public partial class ConfirmationPage : Page
    {
        public ConfirmationPage()
        {
            InitializeComponent();

            CartBoxList.ItemsSource = Core.Context.CartProducts.ToList();
            TotalCostBox.Text = $"Итого: {CalculateTotal().ToString("N0")}";
        }

        private void MakeOrder(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NameBox.Text) && !string.IsNullOrEmpty(MailBox.Text) && !string.IsNullOrEmpty(AddressBox.Text))
            {
                MakeOrder();
            }
            else
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (CartProducts product in Core.Context.CartProducts)
            {
                total += product.TotalCost;
            }
            return total;
        }

        private void MakeOrder()
        {
            Orders order = new Orders()
            {
                FullName = NameBox.Text,
                Address = AddressBox.Text,
                OrderDate = DateTime.Now,
                Email = MailBox.Text,
                TotalCost = CalculateTotal()
            };

            Core.Context.Orders.Add(order);

            foreach (CartProducts cartProduct in Core.Context.CartProducts)
            {
                OrderProducts orderProduct = new OrderProducts()
                {
                    Orders = order,
                    Products = cartProduct.Products,
                    Count = cartProduct.Count,
                    TotalCost = cartProduct.TotalCost
                };
                Core.Context.OrderProducts.Add(orderProduct);
                Core.Context.CartProducts.Remove(cartProduct);
            }

            Core.Context.SaveChanges();

            MessageBox.Show("Заказ оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Asterisk);

            NavigationService.Navigate(new MainPage());
        }
    }
}
