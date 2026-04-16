using System.Windows;
using WpfApp.ViewModels;
namespace WpfApp.Views
{
    public partial class ServiceTypeEditWindow : Window
    {
        public ServiceTypeEditWindow(ServiceTypes item)
        {
            InitializeComponent();
            var vm = (SimpleNameEditViewModel)DataContext;
            vm.Init(item == null ? "" : item.Name, name =>
            {
                if (item == null) { item = new ServiceTypes(); Core.Context.ServiceTypes.Add(item); }
                item.Name = name;
                Core.Context.SaveChanges();
            });
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
