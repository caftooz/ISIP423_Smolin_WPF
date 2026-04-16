using System.Linq;
using System.Windows;

namespace WpfApp.Views
{
    public partial class ServiceEditWindow : Window
    {
        private Services _item;
        private bool _isNew;
        public ServiceEditWindow(Services item)
        {
            InitializeComponent();
            _isNew = item == null;
            _item = item ?? new Services();
            TypeCombo.ItemsSource = Core.Context.ServiceTypes.ToList();
            if (!_isNew)
            {
                NameBox.Text = _item.Name;
                PriceBox.Text = _item.Price.ToString("0.00");
                TypeCombo.SelectedItem = Core.Context.ServiceTypes.FirstOrDefault(t => t.Id == _item.ServiceTypeId);
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text)) { MessageBox.Show("Введите название"); return; }
            if (!decimal.TryParse(PriceBox.Text, out var price)) { MessageBox.Show("Некорректная цена"); return; }
            if (TypeCombo.SelectedItem == null) { MessageBox.Show("Выберите тип"); return; }
            _item.Name = NameBox.Text.Trim();
            _item.Price = price;
            _item.ServiceTypeId = ((ServiceTypes)TypeCombo.SelectedItem).Id;
            if (_isNew) Core.Context.Services.Add(_item);
            Core.Context.SaveChanges();
            DialogResult = true;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
