using System.Windows;
using WpfApp.ViewModels;
namespace WpfApp.Views
{
    public partial class NewAppointmentWindow : Window
    {
        public NewAppointmentWindow()
        {
            InitializeComponent();
            var vm = (NewAppointmentViewModel)DataContext;
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
