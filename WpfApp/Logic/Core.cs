using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Pages;

namespace WpfApp.Logic
{
    internal class Core
    {
        public static DB_pr15Entities Context = new DB_pr15Entities();
        public static assembly_ CurrentAssembly;
        public static List<PartSlot> PartSlots;
    }
    public class PartSlot
    {
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string SelectedPartName { get; set; } = "Не выбрано";
        public int SelectedPartId { get; set; }
        public decimal Price { get; set; } = 0;
        public string ImagePath { get; set; }
        public string PriceString => Price > 0 ? $"{Price:N2} ₽" : "—";
    }
}
