using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace WpfApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для SeatSelectionPage.xaml
    /// </summary>
    public partial class SeatSelectionPage : Page
    {
        private Sessions _currentSession;
        private List<SeatViewModel> _seats = new List<SeatViewModel>();

        public SeatSelectionPage(Sessions session)
        {
            InitializeComponent();
            _currentSession = session;
            LoadSeats();
        }

        private void LoadSeats()
        {
            // Получаем все места в зале этого сеанса
            var allSeatsInHall = Core.Context.Seats
                .Where(s => s.HallId == _currentSession.HallId)
                .OrderBy(s => s.Row).ThenBy(s => s.Number)
                .ToList();

            // Получаем ID всех мест, на которые уже куплены билеты на этот сеанс
            var occupiedSeatIds = Core.Context.Tickets
                .Where(t => t.SessionId == _currentSession.Id)
                .Select(t => t.SeatId)
                .ToList();

            // Создаем список моделей для экрана
            _seats = allSeatsInHall.Select(s => new SeatViewModel
            {
                Id = s.Id,
                Number = s.Number,
                Row = s.Row,
                IsAvailable = !occupiedSeatIds.Contains(s.Id)
            }).ToList();

            int columnsCount = allSeatsInHall.Max(s => s.Number);
            SeatsItemsControl.Tag = columnsCount;

            SeatsItemsControl.ItemsSource = _seats;


            int maxRow = allSeatsInHall.Max(s => s.Row);

            RowsNumbersPanel.Children.Clear();

            for (int i = 1; i <= maxRow; i++)
            {
                TextBlock rowNum = new TextBlock
                {
                    Text = i.ToString(),
                    Foreground = Brushes.Gray,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Height = 34,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                RowsNumbersPanel.Children.Add(rowNum);
            }
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            var selectedSeat = _seats.FirstOrDefault(s => s.IsSelected);
            ConfirmBtn.IsEnabled = selectedSeat != null;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedSeat = _seats.Where(s => s.IsSelected).ToList();
            if (selectedSeat != null && selectedSeat.Count > 0)
            {
                NavigationService.Navigate(new TicketConfirmationPage(_currentSession, selectedSeat));
            }
            else
            {
                MessageBox.Show("Не выбрано ни одно место");
            }
        }
    }
}
