using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class UserEditWindow : Window
    {
        public UserEditWindow(Users user)
        {
            InitializeComponent();
            var vm = (UserEditViewModel)DataContext;
            vm.Init(user);
            vm.RequestClose += r => { DialogResult = r; };
        }
    }
}
