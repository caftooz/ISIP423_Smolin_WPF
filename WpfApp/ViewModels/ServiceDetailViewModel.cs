using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.ViewModels
{
    internal class ServiceDetailViewModel : INotifyPropertyChanged
    {
        private Services _currentService;
        public Services CurrentService
        {
            get => _currentService;
            set { _currentService = value; OnPropertyChanged(); SetServiceMasters(); }
        }

        public ObservableCollection<Users> ServiceMasters { get; } = new ObservableCollection<Users>();
        private void SetServiceMasters()
        {
            var users = Core.Context.MasterServices.ToList()
                                                .Where(ms => ms.ServiceId == CurrentService.Id)
                                                .Select(ms => ms.Users);
            ServiceMasters.Clear();
            foreach (var user in users)
            {
                ServiceMasters.Add(user);
            }
            OnPropertyChanged(nameof(ServiceMasters));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
