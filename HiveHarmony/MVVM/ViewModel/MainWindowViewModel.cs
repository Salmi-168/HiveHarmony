using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveHarmony.MVVM.View;
using HiveHarmony.Tools;

namespace HiveHarmony.MVVM.ViewModel
{
    class MainWindowViewModel : ObservableObject
    {
        private readonly MessengerView _messengerView = new();

        private object _currentView = null!;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            CurrentView = _messengerView;
        }
    }
}
