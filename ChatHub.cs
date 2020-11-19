using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace One2OneChatApp.Hubs
{
    public class ChatHub:Hub
    {

        public static List<User> onlineUsers = new List<User>();
        public static List<ChatInstance> Chats = new List<ChatInstance>();
        public override Task OnConnectedAsync()
        {
            string Id = Context.ConnectionId;
            Clients.Client(Id).SendAsync("GetClientId", Id);
            return base.OnConnectedAsync();
        }
        public void StartChat(string id, string name)
            {
            User user = new User();
            user.Id = id; 
            user.UserName = name ;
            onlineUsers.Add(user);
            Clients.All.SendAsync("AddNewUserToAll", onlineUsers);
        }
        public void GetChatHistory(string thisClientId,string thisClient, string thatClient)
        {
            var ChatHistory = Chats.Where(x=>((x.From.UserName==thisClient || x.To.UserName==thisClient)&& 
            (x.To.UserName==thatClient || x.From.UserName==thatClient))).ToList();
            Clients.Client(thisClientId).SendAsync("ShowChatHistory", ChatHistory);
        }
        public void SendChat( string thisClient, string thatClient,string msg)
        {
            User thisUser = onlineUsers.Where(x => x.UserName == thisClient).FirstOrDefault();
            User thatUser = onlineUsers.Where(x => x.UserName == thatClient).FirstOrDefault();
            ChatInstance chat = new ChatInstance() ;
            chat.From = thisUser;
            chat.To = thatUser; 
            chat.msg = msg;
            chat.dt = DateTime.Now;
            Chats.Add(chat);
            Clients.Client(thisUser.Id).SendAsync("UpdateChatBox", chat);
            Clients.Client(thatUser.Id).SendAsync("ThisUserIsMyThatUser", thisUser.UserName, chat);
        }
    }

    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
    public class ChatInstance
    {
        public User From { get; set; }
        public User To { get; set; }
        public string  msg { get; set; }
        public DateTime dt { get; set; }
    }
}
