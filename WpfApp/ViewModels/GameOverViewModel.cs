using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    public class GameOverViewModel : BaseViewModel
    {
        private int _floorsReached;
        public int FloorsReached
        {
            get => _floorsReached;
            set => SetField(ref _floorsReached, value);
        }

        public string Message => $"Вы погибли на этаже {FloorsReached}.\nПопробуйте ещё раз!";

        public RelayCommand RestartCommand { get; }

        public event Action RestartRequested;

        public GameOverViewModel(int floorsReached)
        {
            FloorsReached = floorsReached;
            RestartCommand = new RelayCommand(() => RestartRequested?.Invoke());
        }
    }
}
