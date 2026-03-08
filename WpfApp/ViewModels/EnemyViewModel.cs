using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Models.Entities;

namespace WpfApp.ViewModels
{
    public class EnemyViewModel : BaseViewModel
    {
        private readonly Enemy _enemy;

        public string Name => _enemy.Name;
        public string ImagePath => _enemy.ImagePath;
        public bool IsAlive => _enemy.IsAlive;

        private int _currentHp;
        public int CurrentHp
        {
            get => _currentHp;
            private set => SetField(ref _currentHp, value);
        }

        private int _maxHp;
        public int MaxHp
        {
            get => _maxHp;
            private set => SetField(ref _maxHp, value);
        }

        public string HpText => $"{CurrentHp}/{MaxHp}";

        public EnemyViewModel(Enemy enemy)
        {
            _enemy = enemy;
            CurrentHp = enemy.CurrentHp;
            MaxHp = enemy.MaxHp;
        }

        public void Refresh()
        {
            CurrentHp = _enemy.CurrentHp;
            MaxHp = _enemy.MaxHp;
            OnPropertyChanged(nameof(IsAlive));
            OnPropertyChanged(nameof(HpText));
        }
    }
}
