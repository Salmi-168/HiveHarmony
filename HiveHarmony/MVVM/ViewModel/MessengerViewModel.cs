using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveHarmony.MVVM.Model;
using HiveHarmony.Tools;

namespace HiveHarmony.MVVM.ViewModel
{
    public class MessengerViewModel : ObservableObject
    {
        public ObservableCollection<ChatListModel> ChatList { get; set; } = [];

        private ChatListModel _selectedChat = null!;
        public ChatListModel SelectedChat
        {
            get => _selectedChat;
            set
            {
                _selectedChat = value;
                OnPropertyChanged();
            }
        }
    }
}
