using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveHarmony.MVVM.Model
{
    public class ChatListModel(string chatName, string lastMessage, string lastMessageTime, string chatImage)
    {
        public string ChatName { get; set; } = chatName;
        public string LastMessage { get; set; } = lastMessage;
        public string LastMessageTime { get; set; } = lastMessageTime;
        public string ChatImage { get; set; } = chatImage;
    }
}
