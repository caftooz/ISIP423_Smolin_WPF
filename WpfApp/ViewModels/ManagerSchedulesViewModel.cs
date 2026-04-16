using System.Collections.Generic;
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
    internal class ManagerSchedulesViewModel : INotifyPropertyChanged
    {
        public List<Users> Masters { get; }
        public ObservableCollection<MasterServices> MasterServicesList { get; } = new ObservableCollection<MasterServices>();
        public ObservableCollection<AppointmentSchedules> Schedules { get; } = new ObservableCollection<AppointmentSchedules>();

        private Users _selectedMaster;
        public Users SelectedMaster
        {
            get => _selectedMaster;
            set { _selectedMaster = value; OnPropertyChanged(); RefreshMasterServices(); }
        }

        private MasterServices _selectedMasterService;
        public MasterServices SelectedMasterService
        {
            get => _selectedMasterService;
            set { _selectedMasterService = value; OnPropertyChanged(); RefreshSchedules(); }
        }

        public ICommand AddScheduleCommand { get; }
        public ICommand DeleteScheduleCommand { get; }

        public ManagerSchedulesViewModel()
        {
            Masters = Core.Context.Users.Where(u => u.UserRoleId == 2).ToList();

            AddScheduleCommand = new RelayCommand(_ =>
            {
                if (SelectedMasterService == null) { MessageBox.Show("Сначала выберите мастера и услугу"); return; }
                var w = new ScheduleAddWindow(SelectedMasterService);
                w.Owner = Application.Current.MainWindow;
                w.ShowDialog();
                RefreshSchedules();
            });

            DeleteScheduleCommand = new RelayCommand(p =>
            {
                var s = p as AppointmentSchedules;
                if (s == null) return;
                Core.Context.AppointmentSchedules.Remove(s);
                Core.Context.SaveChanges();
                RefreshSchedules();
            });
        }

        private void RefreshMasterServices()
        {
            MasterServicesList.Clear();
            Schedules.Clear();
            if (SelectedMaster == null) return;
            foreach (var ms in Core.Context.MasterServices.Where(ms => ms.UserMasterId == SelectedMaster.Id).ToList())
                MasterServicesList.Add(ms);
            OnPropertyChanged(nameof(MasterServicesList));
        }

        private void RefreshSchedules()
        {
            Schedules.Clear();
            if (SelectedMasterService == null) return;
            foreach (var s in Core.Context.AppointmentSchedules
                .Where(a => a.MasterServiceId == SelectedMasterService.Id)
                .ToList()
                .OrderBy(a => a.DayOfWeekId).ThenBy(a => a.Time))
                Schedules.Add(s);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
