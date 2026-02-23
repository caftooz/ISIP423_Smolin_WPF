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
using WpfApp.Logic;

namespace WpfApp.Pages
{
    public partial class PartSelectionPage : Page
    {
        private int _categoryId;
        private List<basepart_> _allParts;

        public PartSelectionPage(int categoryId)
        {
            InitializeComponent();
            _categoryId = categoryId;
            LoadData();
        }

        private void LoadData()
        {
            _allParts = Core.Context.basepart_
                .Where(p => p.parttypeid == _categoryId)
                .ToList();

            var manufacturers = Core.Context.manufacturer_.ToList().Where(m => _allParts.Any(p => p.manufacturerid == m.id)).ToList();
            manufacturers.Insert(0, new manufacturer_ { name = "Все производители", id = 0 });
            CbManufacturer.ItemsSource = manufacturers;
            CbManufacturer.SelectedIndex = 0;

            ApplyFilters();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allParts == null) return;

            var filtered = _allParts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TbSearch.Text))
            {
                filtered = filtered.Where(p => p.name.ToLower().Contains(TbSearch.Text.ToLower()));
            }

            if (CbManufacturer.SelectedItem is manufacturer_ m && m.id != 0)
            {
                filtered = filtered.Where(p => p.manufacturerid == m.id);
            }

            LvParts.ItemsSource = filtered.ToList();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int partId = (int)btn.Tag;

            var slot = Core.PartSlots.FirstOrDefault(ps => ps.CategoryId == _categoryId);
            int i = Core.PartSlots.IndexOf(slot);
            var part = _allParts.First(p => p.id == partId);
            Core.PartSlots[i].SelectedPartName = part.name;
            Core.PartSlots[i].Price = part.price;
            Core.PartSlots[i].SelectedPartId = part.id;

            NavigationService.Navigate(new ConfiguratorPage());
        }
    }
}
