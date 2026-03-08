using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Services
{
    public enum TurnEvent
    {
        Enemy,
        Chest,
        Boss
    }
    public class EventGeneratorService
    {
        private const int BossEveryNFloors = 10;

        public TurnEvent GenerateEvent(int floor)
        {
            if (floor % BossEveryNFloors == 0)
                return TurnEvent.Boss;

            return RandomService.Instance.NextDouble() < 0.5
                ? TurnEvent.Enemy
                : TurnEvent.Chest;
        }
    }
}
