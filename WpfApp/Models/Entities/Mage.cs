using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Services;

namespace WpfApp.Models.Entities
{
    /// <summary>
    /// Особая способность: 15% шанс заморозки (игрок пропускает следующий ход).
    /// </summary>
    public class Mage : Enemy
    {
        protected double FreezeChance { get; set; } = 0.15;

        public Mage()
            : base("Маг", hp: 25, attack: 15, defense: 2, "/Assets/Images/mage.png")
        {
        }

        protected Mage(string name, int hp, int attack, int defense, string imagePath)
            : base(name, hp, attack, defense, imagePath)
        {
        }

        public override string ApplySpecialAbility(Player player, int rawDamage)
        {
            int actualDamage = player.TakeDamage(rawDamage);
            string log = $"{Name} атакует магией. Урон: {actualDamage}";

            bool isFrozen = RandomService.Instance.NextDouble() < FreezeChance;
            if (isFrozen)
            {
                player.Freeze();
                log += "\n❄️ Вы заморожены и пропустите следующий ход!";
            }

            return log;
        }
    }
}
