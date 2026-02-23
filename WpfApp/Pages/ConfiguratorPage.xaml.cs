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
    public partial class ConfiguratorPage : Page
    {
        public ConfiguratorPage()
        {
            InitializeComponent();
            LoadSlots();
            UpdateTotal();
            
        }

        private void LoadSlots()
        {
            if (Core.PartSlots == null)
            {
                Core.PartSlots = new List<PartSlot>();
                foreach (var partType in Core.Context.parttype_)
                {
                    Core.PartSlots.Add(new PartSlot { CategoryName = partType.name, CategoryId = partType.id });
                }
            }

            PartsItemsControl.ItemsSource = Core.PartSlots;
        }

        private void SelectPart_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int categoryId = (int)btn.Tag;

            NavigationService.Navigate(new PartSelectionPage(categoryId));
        }

        private void SaveAssembly_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbAssemblyName.Text) || string.IsNullOrWhiteSpace(TbAuthorName.Text))
            {
                MessageBox.Show("Заполните название и автора сборки!");
                return;
            }

            // Логика сохранения через EDM:
            // 1. Создать объект assembly$
            // 2. Добавить в контекст
            // 3. Сохранить изменения
            MessageBox.Show("Сборка успешно сохранена!");
        }

        public void UpdateTotal()
        {
            decimal total = Core.PartSlots.Sum(s => s.Price);
            TxtTotalPrice.Text = $"{total:N2} ₽";

            CheckCompatibility();
        }

        private void CheckCompatibility()
        {
            // Здесь будет логика сравнения:
            // Slots.First(s => s.CategoryId == 1).SelectedPart...
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить конфигуратор? Все выбранные детали будут удалены.", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                for (int i = 0; i < Core.PartSlots.Count; i++)
                {
                    Core.PartSlots[i].SelectedPartId = 0;
                    Core.PartSlots[i].SelectedPartName = "Не выбрано";
                    Core.PartSlots[i].Price = 0;
                }
                UpdateTotal();
                PartsItemsControl.ItemsSource = null;
                PartsItemsControl.ItemsSource = Core.PartSlots;
            }
        }
    }

}
