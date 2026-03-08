using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Models.Items
{
    public class Weapon : Item
    {
        public int AttackPower { get; private set; }

        public Weapon(string name, int attackPower, string imagePath)
            : base(name, $"Атака: {attackPower}", imagePath)
        {
            AttackPower = attackPower;
        }

        public override string ToString() =>
            $"{Name} (Атака: {AttackPower})";
    }
}
