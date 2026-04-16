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
    internal class ManagerProductsListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Products> Items { get; } = new ObservableCollection<Products>();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); Refresh(); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }

        public ManagerProductsListViewModel()
        {
            Refresh();
            AddCommand = new RelayCommand(_ =>
            {
                var w = new ProductEditWindow(null);
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                Refresh();
            });
            EditCommand = new RelayCommand(p =>
            {
                var prod = p as Products;
                if (prod == null) return;
                var w = new ProductEditWindow(prod);
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                Refresh();
            });
        }

        private void Refresh()
        {
            Items.Clear();
            var q = Core.Context.Products.ToList().AsEnumerable();
            if (!string.IsNullOrEmpty(SearchText))
            {
                var s = SearchText.ToLower();
                q = q.Where(p => p.Name.ToLower().Contains(s));
            }
            foreach (var p in q) Items.Add(p);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
