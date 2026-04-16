using System.Windows;
using WpfApp.ViewModels;
namespace WpfApp.Views
{
    public partial class ManufacturerEditWindow : Window
    {
        public ManufacturerEditWindow(Manufacturers item)
        {
            InitializeComponent();
            var vm = (SimpleNameEditViewModel)DataContext;
            vm.Init(item == null ? "" : item.Name, name =>
            {
                if (item == null) { item = new Manufacturers(); Core.Context.Manufacturers.Add(item); }
                item.Name = name;
                Core.Context.SaveChanges();
            });
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
