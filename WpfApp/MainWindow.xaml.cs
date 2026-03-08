using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp.Services;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private readonly CombatService _combatService = new CombatService();
        private readonly EnemyFactory _enemyFactory = new EnemyFactory();
        private readonly EventGeneratorService _eventGenerator = new EventGeneratorService();
        private readonly LootService _lootService = new LootService();

        public MainWindow()
        {
            InitializeComponent();
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            var vm = new MainMenuViewModel();
            vm.StartGameRequested += ShowGame;

            var view = new MainMenuView { DataContext = vm };
            MainContent.Content = view;
        }

        private void ShowGame()
        {
            var vm = new GameViewModel(_combatService, _enemyFactory, _eventGenerator, _lootService);
            vm.GameOverRequested += ShowGameOver;

            var view = new GameView { DataContext = vm };
            MainContent.Content = view;
        }

        private void ShowGameOver(int floor)
        {
            var vm = new GameOverViewModel(floor);
            vm.RestartRequested += ShowGame;

            var view = new GameOverView { DataContext = vm };
            MainContent.Content = view;
        }
    }
}
