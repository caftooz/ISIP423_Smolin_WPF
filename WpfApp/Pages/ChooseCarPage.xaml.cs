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
    /// Логика взаимодействия для ChooseCarPage.xaml
    /// </summary>
    public partial class ChooseCarPage : Page
    {
        public ChooseCarPage()
        {
            InitializeComponent();

            List<CarModel> CarModels = new List<CarModel>()
            {
                new CarModel("BMW", 300),
                new CarModel("Audi", 100),
                new CarModel("Lamborghini", 10),
            };
            //(DataContext as MainWindow).CarModels;
            CarDataGrid.ItemsSource = CarModels; ;
        }
    }
}
