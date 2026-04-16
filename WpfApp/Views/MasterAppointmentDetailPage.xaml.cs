using System.Windows.Controls;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class MasterAppointmentDetailPage : Page
    {
        public MasterAppointmentDetailPage(Appointments appointment)
        {
            InitializeComponent();
            var vm = (MasterAppointmentDetailViewModel)DataContext;
            vm.Appointment = appointment;
        }
    }
}
