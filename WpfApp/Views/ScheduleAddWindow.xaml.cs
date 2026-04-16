using System.Windows;
using WpfApp.ViewModels;
namespace WpfApp.Views
{
    public partial class ScheduleAddWindow : Window
    {
        public ScheduleAddWindow(MasterServices ms)
        {
            InitializeComponent();
            var vm = (ScheduleAddViewModel)DataContext;
            vm.Init(ms);
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
