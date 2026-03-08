using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Models.Entities;

namespace WpfApp.Services
{
    public class EnemyFactory
    {
        private static readonly List<System.Func<Enemy>> _regularEnemyCreators = new List<System.Func<Enemy>>
        {
            () => new Goblin(),
            () => new Skeleton(),
            () => new Mage()
        };

        private static readonly List<System.Func<Enemy>> _bossCreators = new List<System.Func<Enemy>>
        {
            () => new BossVVG(),
            () => new BossKovalsky(),
            () => new BossArchimag(),
            () => new BossPestov()
        };
        public Enemy CreateRandomEnemy()
        {
            int index = RandomService.Instance.Next(_regularEnemyCreators.Count);
            return _regularEnemyCreators[index]();
        }

        public Enemy CreateRandomBoss()
        {
            int index = RandomService.Instance.Next(_bossCreators.Count);
            return _bossCreators[index]();
        }

        public List<Enemy> CreateEnemyGroup()
        {
            int count = RandomService.Instance.Next(1, 4);
            var group = new List<Enemy>(count);
            for (int i = 0; i < count; i++)
                group.Add(CreateRandomEnemy());
            return group;
        }
        public List<Enemy> CreateBossGroup()
        {
            return new List<Enemy> { CreateRandomBoss() };
        }
    }
}
