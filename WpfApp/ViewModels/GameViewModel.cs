using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Commands;
using WpfApp.Models;
using WpfApp.Models.Entities;
using WpfApp.Models.Items;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly CombatService _combatService;
        private readonly EnemyFactory _enemyFactory;
        private readonly EventGeneratorService _eventGenerator;
        private readonly LootService _lootService;

        private GameState _state;

        public event Action<int> GameOverRequested;  

        private int _floor;
        public int Floor
        {
            get => _floor;
            private set => SetField(ref _floor, value);
        }

        private int _playerHp;
        public int PlayerHp
        {
            get => _playerHp;
            private set => SetField(ref _playerHp, value);
        }

        private int _playerMaxHp;
        public int PlayerMaxHp
        {
            get => _playerMaxHp;
            private set => SetField(ref _playerMaxHp, value);
        }

        private string _weaponText;
        public string WeaponText
        {
            get => _weaponText;
            private set => SetField(ref _weaponText, value);
        }

        private string _armorText;
        public string ArmorText
        {
            get => _armorText;
            private set => SetField(ref _armorText, value);
        }

        private string _weaponImagePath;
        public string WeaponImagePath
        {
            get => _weaponImagePath;
            private set => SetField(ref _weaponImagePath, value);
        }

        private string _armorImagePath;
        public string ArmorImagePath
        {
            get => _armorImagePath;
            private set => SetField(ref _armorImagePath, value);
        }

        private ObservableCollection<EnemyViewModel> _enemies = new ObservableCollection<EnemyViewModel>();
        public ObservableCollection<EnemyViewModel> Enemies
        {
            get => _enemies;
            private set => SetField(ref _enemies, value);
        }

        private string _sceneImagePath;
        public string SceneImagePath
        {
            get => _sceneImagePath;
            private set => SetField(ref _sceneImagePath, value);
        }

        private bool _isChestScene;
        public bool IsChestScene
        {
            get => _isChestScene;
            private set => SetField(ref _isChestScene, value);
        }

        public ObservableCollection<string> EventLog { get; } = new ObservableCollection<string>();

        private bool _isCombatPanelVisible;
        public bool IsCombatPanelVisible
        {
            get => _isCombatPanelVisible;
            private set => SetField(ref _isCombatPanelVisible, value);
        }

        private bool _isChestPanelVisible;
        public bool IsChestPanelVisible
        {
            get => _isChestPanelVisible;
            private set => SetField(ref _isChestPanelVisible, value);
        }

        private bool _isNextFloorVisible;
        public bool IsNextFloorVisible
        {
            get => _isNextFloorVisible;
            private set => SetField(ref _isNextFloorVisible, value);
        }
        private Item _pendingLoot;
        public Item PendingLoot
        {
            get => _pendingLoot;
            private set => SetField(ref _pendingLoot, value);
        }

        private string _lootCompareText;
        public string LootCompareText
        {
            get => _lootCompareText;
            private set => SetField(ref _lootCompareText, value);
        }

        private bool _isLootWeaponOrArmor;
        public bool IsLootWeaponOrArmor
        {
            get => _isLootWeaponOrArmor;
            private set => SetField(ref _isLootWeaponOrArmor, value);
        }
        public RelayCommand AttackCommand { get; }
        public RelayCommand BlockCommand { get; }
        public RelayCommand TakeLootCommand { get; }
        public RelayCommand DropLootCommand { get; }
        public RelayCommand NextFloorCommand { get; }

        public GameViewModel(
            CombatService combatService,
            EnemyFactory enemyFactory,
            EventGeneratorService eventGenerator,
            LootService lootService)
        {
            _combatService = combatService;
            _enemyFactory = enemyFactory;
            _eventGenerator = eventGenerator;
            _lootService = lootService;

            AttackCommand = new RelayCommand(OnAttack, () => IsCombatPanelVisible);
            BlockCommand = new RelayCommand(OnBlock, () => IsCombatPanelVisible);
            TakeLootCommand = new RelayCommand(OnTakeLoot, () => IsChestPanelVisible);
            DropLootCommand = new RelayCommand(OnDropLoot, () => IsChestPanelVisible);
            NextFloorCommand = new RelayCommand(OnNextFloor, () => IsNextFloorVisible);

            StartNewGame();
        }
        public void StartNewGame()
        {
            var player = new Player(maxHp: 100);
            _state = new GameState(player);
            Floor = 1;
            EventLog.Clear();
            AddLog("🗡️ Добро пожаловать в подземелье! Удачи, герой.");
            RefreshPlayerUI();
            GenerateFloorEvent();
        }
        private void GenerateFloorEvent()
        {
            HideAllActionPanels();
            var turnEvent = _eventGenerator.GenerateEvent(_state.Floor);

            switch (turnEvent)
            {
                case TurnEvent.Boss:
                    AddLog($"⚠️ Этаж {_state.Floor}: Появился БОСС!");
                    StartCombat(_enemyFactory.CreateBossGroup());
                    break;

                case TurnEvent.Enemy:
                    AddLog($"👹 Этаж {_state.Floor}: Встреча с врагами!");
                    StartCombat(_enemyFactory.CreateEnemyGroup());
                    break;

                case TurnEvent.Chest:
                    AddLog($"📦 Этаж {_state.Floor}: Вы нашли сундук!");
                    OpenChest();
                    break;
            }
        }

        private void StartCombat(System.Collections.Generic.List<Enemy> enemies)
        {
            _state.CurrentEnemies = enemies;
            _state.IsInCombat = true;

            Enemies = new ObservableCollection<EnemyViewModel>(
                enemies.Select(e => new EnemyViewModel(e)));

            IsChestScene = false;
            IsCombatPanelVisible = true;

            string names = string.Join(", ", enemies.Select(e => e.Name));
            AddLog($"⚔️ Противники: {names}");

            if (_combatService.TryConsumeFreezeSkipTurn(_state.Player))
            {
                AddLog("❄️ Вы заморожены и пропускаете ход!");
                ProcessEnemyTurn(playerChoseBlock: false);
            }
        }

        private void OnAttack() => ExecutePlayerTurn(playerChoseBlock: false);
        private void OnBlock() => ExecutePlayerTurn(playerChoseBlock: true);

        private void ExecutePlayerTurn(bool playerChoseBlock)
        {
            IsCombatPanelVisible = false;

            if (!playerChoseBlock)
            {
                string attackLog = _combatService.PlayerAttacks(_state.Player, _state.CurrentEnemies);
                AddLog(attackLog);
                RefreshEnemiesUI();

                if (_combatService.AreAllEnemiesDead(_state.CurrentEnemies))
                {
                    EndCombatVictory();
                    return;
                }
            }
            else
            {
                AddLog("🛡️ Вы принимаете защитную стойку...");
            }

            ProcessEnemyTurn(playerChoseBlock);
        }

        private void ProcessEnemyTurn(bool playerChoseBlock)
        {
            var result = _combatService.EnemiesAttack(_state.Player, _state.CurrentEnemies, playerChoseBlock);
            AddLog(result.Log);
            RefreshPlayerUI();

            if (result.PlayerDied)
            {
                AddLog("💀 Вы погибли...");
                GameOverRequested?.Invoke(_state.Floor);
                return;
            }

            if (result.AllEnemiesDead)
            {
                EndCombatVictory();
                return;
            }

            IsCombatPanelVisible = true;
        }

        private void EndCombatVictory()
        {
            AddLog("🏆 Все враги повержены! Путь открыт.");
            _state.IsInCombat = false;
            IsNextFloorVisible = true;
        }
        private void OpenChest()
        {
            SceneImagePath = "/Assets/Images/chest.png";
            IsChestScene = true;
            Item loot = _lootService.GenerateLoot();

            if (loot is HealingPotion)
            {
                _state.Player.HealFull();
                RefreshPlayerUI();
                AddLog("💊 Вы нашли зелье лечения и выпили его. HP восстановлено полностью!");
                IsNextFloorVisible = true;
                return;
            }

            PendingLoot = loot;
            IsLootWeaponOrArmor = true;
            LootCompareText = BuildLootCompareText(loot);
            IsChestPanelVisible = true;
        }

        private string BuildLootCompareText(Item newItem)
        {
            if (newItem is Weapon w)
                return $"Найдено: {w}\nТекущее: {_state.Player.EquippedWeapon}";

            if (newItem is Armor a)
                return $"Найдено: {a}\nТекущее: {_state.Player.EquippedArmor}";

            return string.Empty;
        }

        private void OnTakeLoot()
        {
            if (PendingLoot is Weapon w)
            {
                AddLog($"✅ Вы взяли {w.Name} (Атака: {w.AttackPower}).");
                _state.Player.EquipWeapon(w);
            }
            else if (PendingLoot is Armor a)
            {
                AddLog($"✅ Вы надели {a.Name} (Защита: {a.DefenseValue}).");
                _state.Player.EquipArmor(a);
            }

            RefreshPlayerUI();
            FinishChest();
        }

        private void OnDropLoot()
        {
            AddLog($"🗑️ Вы выбросили {PendingLoot?.Name}.");
            FinishChest();
        }

        private void FinishChest()
        {
            PendingLoot = null;
            IsChestPanelVisible = false;
            IsLootWeaponOrArmor = false;
            IsNextFloorVisible = true;
        }
        private void OnNextFloor()
        {
            _state.Floor++;
            Floor = _state.Floor;
            IsNextFloorVisible = false;
            AddLog($"───── Этаж {Floor} ─────");
            GenerateFloorEvent();
        }
        private void RefreshPlayerUI()
        {
            PlayerHp = _state.Player.CurrentHp;
            PlayerMaxHp = _state.Player.MaxHp;
            WeaponText = _state.Player.EquippedWeapon.ToString();
            ArmorText = _state.Player.EquippedArmor.ToString();
            WeaponImagePath = _state.Player.EquippedWeapon.ImagePath;
            ArmorImagePath = _state.Player.EquippedArmor.ImagePath;
        }

        private void RefreshEnemiesUI()
        {
            foreach (var evm in Enemies)
                evm.Refresh();
        }

        private void HideAllActionPanels()
        {
            IsCombatPanelVisible = false;
            IsChestPanelVisible = false;
            IsNextFloorVisible = false;
        }

        private void AddLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            foreach (var line in message.Split('\n'))
                if (!string.IsNullOrWhiteSpace(line))
                    EventLog.Insert(0, line.Trim());
        }
    }
}
