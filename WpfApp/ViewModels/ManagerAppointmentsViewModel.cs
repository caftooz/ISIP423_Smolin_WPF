using System;
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
    internal class ManagerAppointmentsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Appointments> Items { get; } = new ObservableCollection<Appointments>();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); Refresh(); }
        }

        public ICommand AddCommand { get; }
        public ICommand CompleteCommand { get; }
        public ICommand RescheduleCommand { get; }
        public ICommand CancelCommand { get; }

        public ManagerAppointmentsViewModel()
        {
            Refresh();

            AddCommand = new RelayCommand(_ =>
            {
                var w = new NewAppointmentWindow();
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                Refresh();
            });

            CompleteCommand = new RelayCommand(p =>
            {
                var a = p as Appointments;
                if (a == null) return;
                a.IsCompleted = true;
                Core.Context.SaveChanges();
                Refresh();
            });

            RescheduleCommand = new RelayCommand(p =>
            {
                var a = p as Appointments;
                if (a == null) return;
                var w = new RescheduleAppointmentWindow(a);
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                Refresh();
            });

            CancelCommand = new RelayCommand(p =>
            {
                var a = p as Appointments;
                if (a == null) return;
                var result = MessageBox.Show("Отменить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Core.Context.Appointments.Remove(a);
                    Core.Context.SaveChanges();
                    Refresh();
                }
            });
        }

        private void Refresh()
        {
            Items.Clear();
            var q = Core.Context.Appointments.ToList().AsEnumerable();
            if (!string.IsNullOrEmpty(SearchText))
            {
                var s = SearchText.ToLower();
                q = q.Where(a => (a.Users?.LastName ?? "").ToLower().Contains(s)
                              || (a.Users1?.LastName ?? "").ToLower().Contains(s)
                              || (a.Services?.Name ?? "").ToLower().Contains(s));
            }
            foreach (var a in q.OrderByDescending(a => a.AppointmentDateTime))
                Items.Add(a);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
