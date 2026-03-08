using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Services;

namespace WpfApp.Models.Entities
{
    /// <summary>
    /// Особая способность: 20% шанс критического удара (урон ×2).
    /// </summary>
    public class Goblin : Enemy
    {
        protected double CritChance { get; set; } = 0.20;

        public Goblin()
            : base("Гоблин", hp: 30, attack: 12, defense: 3, "/Assets/Images/goblin.png")
        {
        }

        protected Goblin(string name, int hp, int attack, int defense, string imagePath)
            : base(name, hp, attack, defense, imagePath)
        {
        }

        public override string ApplySpecialAbility(Player player, int rawDamage)
        {
            bool isCrit = RandomService.Instance.NextDouble() < CritChance;
            int finalDamage = isCrit ? rawDamage * 2 : rawDamage;

            int actualDamage = player.TakeDamage(finalDamage);

            if (isCrit)
                return $"{Name} наносит КРИТИЧЕСКИЙ удар! Урон: {actualDamage}";

            return $"{Name} атакует. Урон: {actualDamage}";
        }
    }
}
