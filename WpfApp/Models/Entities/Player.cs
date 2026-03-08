using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Models.Items;

namespace WpfApp.Models.Entities
{
    public class Player
    {
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public bool IsAlive => CurrentHp > 0;

        public Weapon EquippedWeapon { get; private set; }
        public Armor EquippedArmor { get; private set; }

        public bool IsFrozen { get; private set; }

        public Player(int maxHp = 100)
        {
            MaxHp = maxHp;
            CurrentHp = maxHp;

            EquippedWeapon = new Weapon("Кулаки", 5, "/Assets/Images/weapon_fist.png");
            EquippedArmor = new Armor("Тряпки", 2, "/Assets/Images/armor_rags.png");
        }

        public int TakeDamage(int damage)
        {
            int actual = System.Math.Max(0, damage);
            CurrentHp = System.Math.Max(0, CurrentHp - actual);
            return actual;
        }

        public void HealFull()
        {
            CurrentHp = MaxHp;
        }

        public void EquipWeapon(Weapon weapon)
        {
            EquippedWeapon = weapon;
        }

        public void EquipArmor(Armor armor)
        {
            EquippedArmor = armor;
        }

        public void Freeze()
        {
            IsFrozen = true;
        }

        public void Unfreeze()
        {
            IsFrozen = false;
        }
    }
}
