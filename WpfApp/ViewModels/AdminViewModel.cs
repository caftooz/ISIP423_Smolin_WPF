using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class AdminViewModel : INotifyPropertyChanged
    {
        public Users CurrentUser => SessionManager.CurrentUser;
        public ObservableCollection<Users> Users { get; } = new ObservableCollection<Users>();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); Refresh(); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand BackCommand { get; }

        public AdminViewModel()
        {
            Refresh();
            AddCommand = new RelayCommand(_ =>
            {
                var w = new UserEditWindow(null);
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                Refresh();
            });
            EditCommand = new RelayCommand(p =>
            {
                var u = p as WpfApp.Users;
                if (u == null) return;
                var w = new UserEditWindow(u);
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                Refresh();
            });
            DeleteCommand = new RelayCommand(p =>
            {
                var u = p as WpfApp.Users;
                if (u == null) return;
                if (u.Id == CurrentUser.Id) { MessageBox.Show("Нельзя удалить самого себя"); return; }
                var result = MessageBox.Show($"Удалить пользователя {u.LastName} {u.FirstName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    u.IsFrozen = true;
                    Core.Context.SaveChanges();
                    Refresh();
                }
            });
            BackCommand = new RelayCommand(_ => MainWindow.GoBack());
            LogoutCommand = new RelayCommand(_ => { SessionManager.Logout(); MainWindow.NavigateTo(new ServicesPage()); });
        }

        private void Refresh()
        {
            Users.Clear();
            var q = Core.Context.Users.ToList().AsEnumerable();
            if (!string.IsNullOrEmpty(SearchText))
            {
                var s = SearchText.ToLower();
                q = q.Where(u => (u.LastName ?? "").ToLower().Contains(s)
                              || (u.FirstName ?? "").ToLower().Contains(s)
                              || (u.PhoneNumber ?? "").Contains(s));
            }
            foreach (var u in q) Users.Add(u);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
