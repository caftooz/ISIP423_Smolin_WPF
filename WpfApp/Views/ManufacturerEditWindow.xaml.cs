using System.Windows;
namespace WpfApp.Views
{
    public partial class ManufacturerEditWindow : Window
    {
        private Manufacturers _item;
        private bool _isNew;
        public ManufacturerEditWindow(Manufacturers item)
        {
            InitializeComponent();
            _isNew = item == null;
            _item = item ?? new Manufacturers();
            if (!_isNew) NameBox.Text = _item.Name;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text)) { MessageBox.Show("Введите название"); return; }
            _item.Name = NameBox.Text.Trim();
            if (_isNew) Core.Context.Manufacturers.Add(_item);
            Core.Context.SaveChanges();
            DialogResult = true;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
