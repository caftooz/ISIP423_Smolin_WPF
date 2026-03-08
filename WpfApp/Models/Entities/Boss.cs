using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Services;

namespace WpfApp.Models.Entities
{
    /// <summary>
    /// ВВГ — босс-гоблин. Множители: HP×2.0, Атака×1.5, Защита×1.2.
    /// Доп. особенность: шанс крита +10% (итого 30%).
    /// </summary>
    public class BossVVG : Goblin
    {
        public BossVVG()
            : base(
                name: "ВВГ",
                hp: (int)Math.Round(30 * 2.0),
                attack: (int)Math.Round(12 * 1.5),
                defense: (int)Math.Round(3 * 1.2),
                imagePath: "/Assets/Images/goblin.png")
        {
            CritChance = 0.30;
        }
    }

    /// <summary>
    /// Ковальский — босс-скелет. Множители: HP×2.5, Атака×1.3, Защита×1.4.
    /// Нет дополнительных особенностей (наследует игнор брони от Skeleton).
    /// </summary>
    public class BossKovalsky : Skeleton
    {
        public BossKovalsky()
            : base(
                name: "Ковальский",
                hp: (int)Math.Round(40 * 2.5),
                attack: (int)Math.Round(10 * 1.3),
                defense: (int)Math.Round(5 * 1.4),
                imagePath: "/Assets/Images/skeleton.png")
        {
        }
    }

    /// <summary>
    /// Архимаг C++ — босс-маг. Множители: HP×1.8, Атака×1.6, Защита×1.1.
    /// Доп. особенность: шанс заморозки +10% (итого 25%).
    /// </summary>
    public class BossArchimag : Mage
    {
        public BossArchimag()
            : base(
                name: "Архимаг C++",
                hp: (int)Math.Round(25 * 1.8),
                attack: (int)Math.Round(15 * 1.6),
                defense: (int)Math.Round(2 * 1.1),
                imagePath: "/Assets/Images/mage.png")
        {
            FreezeChance = 0.25;
        }
    }

    /// <summary>
    /// Пестов С–– — босс-скелет. Множители: HP×1.3, Атака×1.8, Защита×0.6.
    /// Доп. особенность: +15% шанс заморозки (унаследован от Mage через переопределение).
    /// Реализует собственную способность: игнор брони + шанс заморозки.
    /// </summary>
    public class BossPestov : Skeleton
    {
        private const double FreezeChance = 0.15;

        public BossPestov()
            : base(
                name: "Пестов С––",
                hp: (int)Math.Round(40 * 1.3),
                attack: (int)Math.Round(10 * 1.8),
                defense: (int)Math.Round(5 * 0.6),
                imagePath: "/Assets/Images/skeleton.png")
        {
        }

        public override string ApplySpecialAbility(Player player, int rawDamage)
        {
            string log = base.ApplySpecialAbility(player, rawDamage);

            bool isFrozen = RandomService.Instance.NextDouble() < FreezeChance;
            if (isFrozen)
            {
                player.Freeze();
                log += "\n❄️ Пестов С–– замораживает вас! Следующий ход пропущен.";
            }

            return log;
        }
    }
}
