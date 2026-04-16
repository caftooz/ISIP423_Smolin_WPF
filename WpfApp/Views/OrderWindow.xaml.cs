using System.Collections.Generic;
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class OrderWindow : Window
    {
        public OrderWindow(List<CartItemViewModel> items)
        {
            InitializeComponent();
            var vm = (OrderWindowViewModel)DataContext;
            vm.Init(items);
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
