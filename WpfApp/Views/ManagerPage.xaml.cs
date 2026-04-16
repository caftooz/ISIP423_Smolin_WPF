using System.Windows.Controls;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class ManagerPage : Page
    {
        public static Frame ManagerFrameInstance;

        public ManagerPage()
        {
            InitializeComponent();
            ManagerFrameInstance = ManagerFrame;
            var vm = (ManagerViewModel)DataContext;
            vm.SetFrame(ManagerFrame);
            ManagerFrame.Navigate(new ManagerAppointmentsPage());
        }
    }
}
