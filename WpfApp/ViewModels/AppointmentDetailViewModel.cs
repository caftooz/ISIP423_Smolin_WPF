using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using WpfApp.Commands;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    internal class AppointmentDetailViewModel : INotifyPropertyChanged
    {
        private MasterServices _masterService;
        public MasterServices MasterService
        {
            get => _masterService;
            set { _masterService = value; OnPropertyChanged(); }
        }

        private DateTime _dateTime;
        public DateTime DateTime
        {
            get => _dateTime;
            set { _dateTime = value; OnPropertyChanged(); }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }

        public List<PaymentMethods> PaymentMethods => Core.Context.PaymentMethods.ToList();

        private PaymentMethods _selectedPaymentMethod;
        public PaymentMethods SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set { _selectedPaymentMethod = value; OnPropertyChanged(); }
        }

        private string _errorMessage = null;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand MakeAnAppointment { get; }

        public AppointmentDetailViewModel()
        {
            MakeAnAppointment = new RelayCommand(_ =>
            {
                ErrorMessage = null;
                if (SelectedPaymentMethod == null)
                {
                    ErrorMessage = "Выберите метод оплаты";
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Записаться?", "Запись на услугу", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var appointment = new Appointments
                    {
                        ServiceId = MasterService.ServiceId,
                        UserClientId = SessionManager.CurrentUser.Id,
                        UserMasterId = MasterService.UserMasterId,
                        AppointmentDateTime = DateTime,
                        CreatedDateTime = DateTime.Now,
                        PaymentMethodId = SelectedPaymentMethod.Id,
                        Comment = this.Comment,
                        IsCompleted = false
                    };

                    Core.Context.Appointments.Add(appointment);
                    Core.Context.SaveChanges();

                    MessageBox.Show("Вы успешно записались", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    MainWindow.NavigateTo(new ServicesPage());
                }
            });
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
