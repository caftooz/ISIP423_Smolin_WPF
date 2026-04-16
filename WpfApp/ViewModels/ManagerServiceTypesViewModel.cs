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
    internal class ManagerServiceTypesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ServiceTypes> Items { get; } = new ObservableCollection<ServiceTypes>();
        private string _searchText;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); Refresh(); } }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }

        public ManagerServiceTypesViewModel()
        {
            Refresh();
            AddCommand = new RelayCommand(_ => { var w = new ServiceTypeEditWindow(null); w.Owner = Application.Current.MainWindow; w.ShowDialog(); Refresh(); });
            EditCommand = new RelayCommand(p => { var i = p as ServiceTypes; if (i == null) return; var w = new ServiceTypeEditWindow(i); w.Owner = Application.Current.MainWindow; w.ShowDialog(); Refresh(); });
        }
        private void Refresh()
        {
            Items.Clear();
            var q = Core.Context.ServiceTypes.ToList().AsEnumerable();
            if (!string.IsNullOrEmpty(SearchText)) q = q.Where(i => i.Name.ToLower().Contains(SearchText.ToLower()));
            foreach (var i in q) Items.Add(i);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
