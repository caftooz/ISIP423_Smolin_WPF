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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private List<Products> _allProducts;
        public MainPage()
        {
            InitializeComponent();

            _allProducts = Core.Context.Products.ToList();
            ProductListBox.ItemsSource = _allProducts;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Products product = button.DataContext as Products;
            if (product != null)
            {
                CartProducts cartProduct = Core.Context.CartProducts.FirstOrDefault(p => p.ProductId == product.ProductId);
                if (cartProduct != null)
                {
                    cartProduct.Count++;
                    cartProduct.TotalCost += product.Cost;
                }
                else
                {
                    cartProduct = new CartProducts()
                    {
                        Products = product,
                        Count = 1,
                        TotalCost = product.Cost
                    };
                    Core.Context.CartProducts.Add(cartProduct);
                }
                Core.Context.SaveChanges();
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }
    }
}
