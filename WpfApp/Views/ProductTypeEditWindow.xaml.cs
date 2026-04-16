using System.Windows;
using WpfApp.ViewModels;
namespace WpfApp.Views
{
    public partial class ProductTypeEditWindow : Window
    {
        public ProductTypeEditWindow(ProductTypes item)
        {
            InitializeComponent();
            var vm = (SimpleNameEditViewModel)DataContext;
            vm.Init(item == null ? "" : item.Name, name =>
            {
                if (item == null) { item = new ProductTypes(); Core.Context.ProductTypes.Add(item); }
                item.Name = name;
                Core.Context.SaveChanges();
            });
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
