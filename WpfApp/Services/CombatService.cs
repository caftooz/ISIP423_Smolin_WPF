using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Models.Entities;

namespace WpfApp.Services
{
    public class CombatTurnResult
    {
        public string Log { get; set; } = string.Empty;
        public bool PlayerDied { get; set; }
        public bool AllEnemiesDead { get; set; }
        public bool PlayerFrozenThisTurn { get; set; }
    }

    public class CombatService
    {
        /// <summary>Шанс полного уклонения при выборе Защиты</summary>
        private const double DodgeChanceOnBlock = 0.40;

        /// <summary>Нижняя граница снижения урона блоком (доля от DefenseValue)</summary>
        private const double BlockReductionMin = 0.70;

        /// <summary>Верхняя граница снижения урона блоком (доля от DefenseValue)</summary>
        private const double BlockReductionMax = 1.00;

        public string PlayerAttacks(Player player, List<Enemy> enemies)
        {
            var log = new StringBuilder();
            int weaponDamage = player.EquippedWeapon.AttackPower;

            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;

                int actualDamage = enemy.TakeDamage(weaponDamage);
                log.AppendLine($"⚔️ Вы атакуете {enemy.Name} — урон: {actualDamage}. " +
                               $"HP врага: {Math.Max(0, enemy.CurrentHp)}/{enemy.MaxHp}");

                if (!enemy.IsAlive)
                    log.AppendLine($"💀 {enemy.Name} повержен!");
            }

            return log.ToString().TrimEnd();
        }

        public CombatTurnResult EnemiesAttack(
            Player player,
            List<Enemy> enemies,
            bool playerChoseBlock)
        {
            var result = new CombatTurnResult();
            var log = new StringBuilder();

            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;
                if (!player.IsAlive) break;

                int rawDamage = CalculateRawDamage(enemy, player, playerChoseBlock, log);

                if (rawDamage > 0)
                {
                    string abilityLog = enemy.ApplySpecialAbility(player, rawDamage);
                    log.AppendLine(abilityLog);

                    if (player.IsFrozen)
                        result.PlayerFrozenThisTurn = true;
                }
            }

            result.Log = log.ToString().TrimEnd();
            result.PlayerDied = !player.IsAlive;
            result.AllEnemiesDead = AreAllEnemiesDead(enemies);

            return result;
        }

        public bool TryConsumeFreezeSkipTurn(Player player)
        {
            if (!player.IsFrozen) return false;
            player.Unfreeze();
            return true;
        }
        public bool AreAllEnemiesDead(List<Enemy> enemies)
        {
            foreach (var e in enemies)
                if (e.IsAlive) return false;
            return true;
        }

        private int CalculateRawDamage(
            Enemy enemy,
            Player player,
            bool playerChoseBlock,
            StringBuilder log)
        {
            int rawDamage = enemy.Attack;

            if (!playerChoseBlock)
                return rawDamage;

            bool dodged = RandomService.Instance.NextDouble() < DodgeChanceOnBlock;
            if (dodged)
            {
                log.AppendLine($"🛡️ Вы уклонились от атаки {enemy.Name}!");
                return 0;
            }

            double reductionFactor = BlockReductionMin +
                RandomService.Instance.NextDouble() * (BlockReductionMax - BlockReductionMin);

            int reduction = (int)Math.Round(player.EquippedArmor.DefenseValue * reductionFactor);
            int blockedDamage = Math.Max(1, rawDamage - reduction);

            log.AppendLine($"🛡️ Блок! Снижение урона от {enemy.Name}: -{reduction}.");
            return blockedDamage;
        }
    }
}
