using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Models.Entities;

namespace WpfApp.Models
{
    public class GameState
    {
        public int Floor { get; set; } = 1;

        public Player Player { get; set; }

        public List<Enemy> CurrentEnemies { get; set; } = new List<Enemy>();

        public bool IsInCombat { get; set; } = false;

        public bool IsChestOpen { get; set; } = false;

        public bool IsGameOver { get; set; } = false;

        public bool AwaitingPlayerAction { get; set; } = false;

        public int CurrentEnemyIndex { get; set; } = 0;

        public bool PlayerChoseBlock { get; set; } = false;

        public GameState(Player player)
        {
            Player = player;
        }
    }
}
