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
    internal class ManagerManufacturersViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Manufacturers> Items { get; } = new ObservableCollection<Manufacturers>();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); Refresh(); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }

        public ManagerManufacturersViewModel()
        {
            Refresh();
            AddCommand = new RelayCommand(_ =>
            {
                var w = new ManufacturerEditWindow(null);
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                Refresh();
            });
            EditCommand = new RelayCommand(p =>
            {
                var m = p as Manufacturers;
                if (m == null) return;
                var w = new ManufacturerEditWindow(m);
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                Refresh();
            });
        }

        private void Refresh()
        {
            Items.Clear();
            var q = Core.Context.Manufacturers.ToList().AsEnumerable();
            if (!string.IsNullOrEmpty(SearchText))
                q = q.Where(m => m.Name.ToLower().Contains(SearchText.ToLower()));
            foreach (var m in q) Items.Add(m);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
