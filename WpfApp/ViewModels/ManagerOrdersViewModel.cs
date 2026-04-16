using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    internal class ManagerOrdersViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Orders> Items { get; } = new ObservableCollection<Orders>();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); Refresh(); }
        }

        public ICommand CompleteCommand { get; }

        public ManagerOrdersViewModel()
        {
            Refresh();
            CompleteCommand = new RelayCommand(p =>
            {
                var o = p as Orders;
                if (o == null) return;
                o.IsCompleted = true;
                Core.Context.SaveChanges();
                Refresh();
            });
        }

        private void Refresh()
        {
            Items.Clear();
            var q = Core.Context.Orders.ToList().AsEnumerable();
            if (!string.IsNullOrEmpty(SearchText))
            {
                var s = SearchText.ToLower();
                q = q.Where(o => (o.Users?.LastName ?? "").ToLower().Contains(s));
            }
            foreach (var o in q.OrderByDescending(o => o.OrderDateTime))
                Items.Add(o);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
