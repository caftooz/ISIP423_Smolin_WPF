using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Commands;

namespace WpfApp.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        public RelayCommand StartGameCommand { get; }

        public event Action StartGameRequested;

        public MainMenuViewModel()
        {
            StartGameCommand = new RelayCommand(() => StartGameRequested?.Invoke());
        }
    }
}
