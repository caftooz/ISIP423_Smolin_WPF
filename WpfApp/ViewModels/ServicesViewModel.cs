using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class ServicesViewModel : INotifyPropertyChanged
    {
        private string _findText;
        public string FindText
        {
            get => _findText;
            set { _findText = value; OnPropertyChanged(); UpdateServices(); }
        }

        private Services _selectedService;
        public Services SelectedService
        {
            get => _selectedService;
            set
            {
                _selectedService = value;
                if (value != null)
                    MainWindow.NavigateTo(new ServiceDetailPage(value));
            }
        }

        private List<FilterChoice<Users>> _masters;
        public List<FilterChoice<Users>> Masters => _masters;


        private List<FilterChoice<ServiceTypes>> _serviceTypes;
        public List<FilterChoice<ServiceTypes>> ServiceTypes => _serviceTypes;

        public ObservableCollection<Services> Services { get; } = new ObservableCollection<Services>();

        public ServicesViewModel()
        {
            _masters = Core.Context.Users.Where(u => u.UserRoleId == 2).ToList().Select(u => new FilterChoice<Users>(u, UpdateServices)).ToList();
            _serviceTypes = Core.Context.ServiceTypes.ToList().Select(s => new FilterChoice<ServiceTypes>(s, UpdateServices)).ToList();
            UpdateServices();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void UpdateServices()
        {
            var services = Core.Context.Services.ToList().AsQueryable();

            if (_serviceTypes.Any(s => s.IsActive))
            {
                var activeTypes = _serviceTypes.Where(st => st.IsActive);
                services = services.Where(s => activeTypes.Any(st => s.ServiceTypeId == st.Item.Id));
            }
            if (_masters.Any(m => m.IsActive))
            {
                var activeMasterIds = _masters.Where(m => m.IsActive).Select(m => m.Item.Id).ToHashSet();
                var allowedServiceIds = Core.Context.MasterServices
                    .Where(ms => activeMasterIds.Contains(ms.UserMasterId))
                    .Select(ms => ms.ServiceId)
                    .ToHashSet();
                services = services.Where(s => allowedServiceIds.Contains(s.Id));
            }
            if (!string.IsNullOrEmpty(FindText))
            {
                services = services.Where(s => s.Name.ToLower().Contains(FindText.ToLower()));
            }

            Services.Clear();
            foreach (var service in services)
            {
                Services.Add(service);
            }
            OnPropertyChanged(nameof(Services));
        }

        public class FilterChoice<T>
        {
            public T Item { get; }
            private bool _isActive;
            private readonly Action _onChanged;

            public bool IsActive
            {
                get => _isActive;
                set { _isActive = value; _onChanged?.Invoke(); }
            }

            public FilterChoice(T item, Action onChanged)
            {
                Item = item;
                _onChanged = onChanged;
            }
        }
    }
}
