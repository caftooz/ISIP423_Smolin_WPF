using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Models.Items
{
    public class Armor : Item
    {
        public int DefenseValue { get; private set; }

        public Armor(string name, int defenseValue, string imagePath)
            : base(name, $"Защита: {defenseValue}", imagePath)
        {
            DefenseValue = defenseValue;
        }

        public override string ToString() =>
            $"{Name} (Защита: {DefenseValue})";
    }
}
