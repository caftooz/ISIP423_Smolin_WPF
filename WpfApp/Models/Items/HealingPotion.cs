using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Models.Items
{
    public class HealingPotion : Item
    {
        public HealingPotion()
            : base("Зелье лечения", "Полностью восстанавливает HP", "...")
        {
        }
    }
}
