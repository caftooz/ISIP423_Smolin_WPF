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
    internal class ManagerServicesListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Services> Items { get; } = new ObservableCollection<Services>();
        private string _searchText;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); Refresh(); } }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }

        public ManagerServicesListViewModel()
        {
            Refresh();
            AddCommand = new RelayCommand(_ => { var w = new ServiceEditWindow(null); w.Owner = Application.Current.MainWindow; w.ShowDialog(); Refresh(); });
            EditCommand = new RelayCommand(p => { var s = p as Services; if (s == null) return; var w = new ServiceEditWindow(s); w.Owner = Application.Current.MainWindow; w.ShowDialog(); Refresh(); });
        }
        private void Refresh()
        {
            Items.Clear();
            var q = Core.Context.Services.ToList().AsEnumerable();
            if (!string.IsNullOrEmpty(SearchText)) q = q.Where(i => i.Name.ToLower().Contains(SearchText.ToLower()));
            foreach (var i in q) Items.Add(i);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
