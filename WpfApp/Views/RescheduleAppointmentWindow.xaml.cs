using System.Windows;
using WpfApp.ViewModels;
namespace WpfApp.Views
{
    public partial class RescheduleAppointmentWindow : Window
    {
        public RescheduleAppointmentWindow(Appointments appointment)
        {
            InitializeComponent();
            var vm = (RescheduleViewModel)DataContext;
            vm.Init(appointment);
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
