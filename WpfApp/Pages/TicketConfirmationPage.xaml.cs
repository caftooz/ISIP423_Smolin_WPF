using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для TicketConfirmationPage.xaml
    /// </summary>
    public partial class TicketConfirmationPage : Page
    {
        Sessions _session;
        List<SeatViewModel> _seats;
        public TicketConfirmationPage(Sessions session, List<SeatViewModel> seats)
        {
            InitializeComponent();
            _session = session;
            _seats = seats;

            this.DataContext = _session;

            SeatsListBox.ItemsSource = _seats;
            PriceText.Text = _session.Price * _seats.Count + " руб.";
            PriceInfo.Text = _session.Price + " руб.";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_seats == null) return;
            foreach (var seat in _seats)
            {
                Tickets ticket = new Tickets() 
                {
                    Sessions = _session,
                    SeatId = seat.Id,
                    UserId = Core.UserID,
                    PurchaseDate = DateTime.Now
                };

                Core.Context.Tickets.Add(ticket);
                Core.Context.SaveChanges();

                NavigationService.Navigate(new ProfilePage());

            }
        }
    }
}
