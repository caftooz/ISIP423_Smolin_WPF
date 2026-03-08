using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Models.Items;

namespace WpfApp.Services
{
    public enum LootType
    {
        Weapon,
        Armor,
        HealingPotion
    }

    public class LootService
    {

        private static readonly List<Weapon> _weapons = new List<Weapon>
        {
            new Weapon("Ржавый кинжал",    7,  "/Assets/Images/weapon_dagger.png"),
            new Weapon("Короткий меч",     12, "/Assets/Images/weapon_shortsword.png"),
            new Weapon("Боевой топор",     16, "/Assets/Images/weapon_axe.png"),
            new Weapon("Длинный меч",      20, "/Assets/Images/weapon_sword.png"),
            new Weapon("Двуручный молот",  25, "/Assets/Images/weapon_hammer.png")
        };

        private static readonly List<Armor> _armors = new List<Armor>
        {
            new Armor("Кожаный нагрудник", 5,  "/Assets/Images/armor_leather.png"),
            new Armor("Кольчуга",          10, "/Assets/Images/armor_golden.png"),
            new Armor("Железный нагрудник",     16, "/Assets/Images/armor_iron.png"),
            new Armor("Алмазный нагрудник",22, "/Assets/Images/armor_diamond.png")
        };

        public LootType RollLootType()
        {
            int roll = RandomService.Instance.Next(3);
            LootType type;
            switch(roll)
            {
                case 0:
                    type = LootType.Weapon;
                    break;
                case 1:
                    type = LootType.Armor;
                    break;
                case 2:
                    type = LootType.HealingPotion;
                    break;
                default:
                    return default(LootType);
            }
            return type;
        }

        public Weapon GetRandomWeapon()
        {
            int index = RandomService.Instance.Next(_weapons.Count);
            return _weapons[index];
        }

        public Armor GetRandomArmor()
        {
            int index = RandomService.Instance.Next(_armors.Count);
            return _armors[index];
        }

        public HealingPotion GetHealingPotion()
        {
            return new HealingPotion();
        }
        public Item GenerateLoot()
        {
            switch (RollLootType())
            {
                case LootType.Weapon:
                        return (Item)GetRandomWeapon();
                    case LootType.Armor:
                        return GetRandomArmor();
                    default:
                        return GetHealingPotion();
            }
        }
    }
}
