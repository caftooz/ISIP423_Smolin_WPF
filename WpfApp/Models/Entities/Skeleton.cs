using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Models.Entities
{
    /// <summary>
    /// Особая способность: игнорирует защиту игрока полностью.
    /// </summary>
    public class Skeleton : Enemy
    {
        public Skeleton()
            : base("Скелет", hp: 40, attack: 10, defense: 5, "/Assets/Images/skeleton.png")
        {
        }

        protected Skeleton(string name, int hp, int attack, int defense, string imagePath)
            : base(name, hp, attack, defense, imagePath)
        {
        }
        public override string ApplySpecialAbility(Player player, int rawDamage)
        {
            int actualDamage = player.TakeDamage(rawDamage);
            return $"{Name} игнорирует вашу броню! Урон: {actualDamage}";
        }
    }
}
