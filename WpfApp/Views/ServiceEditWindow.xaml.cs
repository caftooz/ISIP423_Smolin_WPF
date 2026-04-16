using System.Windows;
using WpfApp.ViewModels;
namespace WpfApp.Views
{
    public partial class ServiceEditWindow : Window
    {
        public ServiceEditWindow(Services item)
        {
            InitializeComponent();
            var vm = (ServiceEditWindowViewModel)DataContext;
            vm.Init(item);
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
