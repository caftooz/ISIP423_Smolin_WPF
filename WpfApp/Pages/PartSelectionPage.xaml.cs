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
using System.Xml.Linq;
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

            if (Core.PartSlots.FirstOrDefault(ps => ps.CategoryId == _categoryId)?.SelectedPartId is int selectedPartId)
            {
                var selectedPart = _allParts.FirstOrDefault(p => p.id == selectedPartId);
                if (selectedPart != null)
                {
                    LvParts.SelectedItem = selectedPart;
                    LvParts.ScrollIntoView(selectedPart);
                }
            }
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

            AddPart(partId);
        }

        private void AddPart(int partId)
        {
            var slot = Core.PartSlots.FirstOrDefault(ps => ps.CategoryId == _categoryId);
            int i = Core.PartSlots.IndexOf(slot);
            var part = _allParts.First(p => p.id == partId);
            Core.PartSlots[i].SelectedPartName = part.name;
            Core.PartSlots[i].Price = part.price;
            Core.PartSlots[i].SelectedPartId = part.id;
            Core.PartSlots[i].ImagePath = part.image;

            Core.CurrentAssembly = null;

            NavigationService.Navigate(new ConfiguratorPage());
        }

        private void BtnChooseSelect_Click(object sender, RoutedEventArgs e)
        {
            var item = LvParts.SelectedItem as basepart_;
            AddPart(item.id);
        }

        private void LvParts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPart = LvParts.SelectedItem as basepart_;
            if (selectedPart == null) return;

            OpenInfo(selectedPart);
        }

        private void OpenInfo(basepart_ selectedPart)
        {
            DetailsColumn.Width = new GridLength(400);
            DetailsPanel.Visibility = Visibility.Visible;

            TxtDetailName.Text = selectedPart.name;
            TxtDetailPrice.Text = $"{selectedPart.price:N2} ₽";
            ImgDetail.Source = new BitmapImage(new Uri(selectedPart.image, UriKind.RelativeOrAbsolute));


            IcSpecs.ItemsSource = LoadSpecs(selectedPart);
        }

        private Dictionary<string, string> LoadSpecs(basepart_ part)
        {
            var specs = new Dictionary<string, string>();

            switch (part.parttypeid)
            {
                case 1: // CPU
                    var cpu = Core.Context.cpu_.Find(part.id);
                    if (cpu != null)
                    {
                        specs.Add("Сокет", cpu.socket_?.name ?? "н/д");
                        specs.Add("Ядра", cpu.numberofcores.ToString());
                        specs.Add("Базовая частота", cpu.basecorefrequency.ToString() + " MHz");
                        specs.Add("Максимальная частота", cpu.maxcorefrequency.ToString() + " MHz");
                        specs.Add("Кэш L3", cpu.cachel3.ToString() + " МБ");
                        specs.Add("Встроенный ГП", cpu.igpu_?.name?.ToString() ?? "нет");
                        specs.Add("Потребление", cpu.thermalpower.ToString() + " Вт");
                    }
                    break;

                case 2: // GPU
                    var gpu = Core.Context.gpu_.Find(part.id);
                    if (gpu != null)
                    {
                        var sb = new StringBuilder();
                        foreach (var c in gpu.videoconnectorgpu_)
                        {
                            sb.Append(c.videoconnector_.name.ToString());
                            sb.Append(", ");
                        }
                        specs.Add("Объем видео памяти", gpu.videomemory.ToString() + " ГБ");
                        specs.Add("Мин. БП", gpu.recommendpower.ToString() + " Вт");
                        specs.Add("Интерфейс", gpu.gpuinterface_?.name?.ToString() ?? "н/д");
                        specs.Add("Частота", gpu.chipfrequency.ToString() + " MHz");
                        specs.Add("Разъемы", sb.ToString());
                    }
                    break;

                case 3: // Ram
                    var ram = Core.Context.ram_.Find(part.id);
                    if (ram != null)
                    {
                        specs.Add("Тип", ram.memorytype_?.name ?? "н/д");
                        specs.Add("Объем", ram.capacity.ToString() + " ГБ");
                        specs.Add("Кол-во", ram.count.ToString());
                        specs.Add("Частота", ram.ghz.ToString() + " GHz");
                        specs.Add("Тайминги", ram.timings.ToString());
                    }
                    break;

                case 4: // Mother
                    var mb = Core.Context.motherboard_.Find(part.id);
                    if (mb != null)
                    {
                        specs.Add("Сокет", mb.socket_?.name ?? "н/д");
                        specs.Add("Формфактор", mb.formfactor_?.name ?? "н/д");
                        specs.Add("Слотов памяти", mb.memoryslots.ToString());
                        specs.Add("Тип памяти", mb.memorytype_?.name?.ToString() ?? "н/д");
                        specs.Add("Слотов PCI", mb.pcislots.ToString());
                        specs.Add("Слотов SATA", mb.sataports.ToString());
                        specs.Add("Разъемов USB", mb.usbports.ToString());
                    }
                    break;

                case 5: // Case
                    var cs = Core.Context.case_.Find(part.id);
                    if (cs != null)
                    {
                        specs.Add("Размер", cs.casesize_?.name ?? "н/д");
                        specs.Add("Слоты расширения", cs.expansionslots.ToString());
                        specs.Add("Вентиляторы", cs.fans.ToString());
                    }
                    break;

                case 6: // Power
                    var pw = Core.Context.powersupply_.Find(part.id);
                    if (pw != null)
                    {
                        specs.Add("Мощность", pw.power.ToString() + " Вт");
                        specs.Add("Габариты", pw.fandimension_?.name?.ToString() ?? "н/д");
                        specs.Add("Сертификация", pw.certificate_?.name?.ToString() ?? "н/д");
                    }
                    break;

                case 7: // Cooler
                    var pc = Core.Context.processorcooler_.Find(part.id);
                    if (pc != null)
                    {
                        specs.Add("Габариты", pc.fandimension_?.name?.ToString() ?? "н/д");
                        specs.Add("Тепловые трубки", pc.heatpipes.ToString());
                        specs.Add("Мин скорость", pc.minspeed.ToString() + " об/мин");
                        specs.Add("Макс скорость", pc.maxspeed.ToString() + " об/мин");
                        specs.Add("Уровень шума", pc.noiselevel.ToString() + " дБ");
                    }
                    break;

                case 8: // Storage
                    var st = Core.Context.storagedevice_.Find(part.id);
                    if (st != null)
                    {
                        specs.Add("Объем", st.capacity.ToString() + " ГБ");
                        specs.Add("Интерфейс", st.storagedeviceinterface_?.name?.ToString() ?? "н/д");
                        specs.Add("Тип", st.storagedevicetype_?.name?.ToString() ?? "н/д");
                    }
                    var hdd = Core.Context.hdd_.Find(part.id);
                    if (hdd != null)
                    {
                        specs.Add("Скорость врщения", hdd.rotationspeed.ToString() + " об/мин");
                    }
                    var ssd = Core.Context.ssd_.Find(part.id);
                    if (ssd != null)
                    {
                        specs.Add("TBW ", ssd.tbw.ToString() + " ТБ");
                    }
                    break;
            }
            return specs;
        }

        private void CloseDetails_Click(object sender, RoutedEventArgs e)
        {
            DetailsColumn.Width = new GridLength(0);
            DetailsPanel.Visibility = Visibility.Collapsed;
        }
    }
}
