using ChatService.Models;
using ChatService.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgammonChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private DataContext _context;
        private readonly IDictionary<string, UserConnection> _connections;
        public ChatHub(IDictionary<string, UserConnection> connections, DataContext context)
        {
            _botUser = "MyChat";
            _connections = connections;
            _context = context;
        }

        //override from hub and disconnect connection
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.ChatConnection)
                    .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");
                SendConnectedUsers(userConnection.ChatConnection);
            }
            return base.OnDisconnectedAsync(exception);
        }


        //get details about user, the message context, send
        public async Task SendMessage  (string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.ChatConnection)
                 .SendAsync("ReceiveMessage", userConnection.User, message);
                EntityEntry<Message> entityEntry = await _context.Messages.AddAsync(new Message { DateSent = DateTime.Now, Content = message, Id = new Guid() });
                var checker = _context.SaveChanges();
            }
        }


        //get details about user connection, enter the chat
        public async Task JoinChat(UserConnection userConnection)
        {
            
            //to notify connection to the chat
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatConnection);

            _connections[Context.ConnectionId] = userConnection;

            
            await Clients.Group(userConnection.ChatConnection).SendAsync("ReceiveMessage", _botUser,
                $"{userConnection.User} joined {userConnection.ChatConnection}");

            await SendConnectedUsers(userConnection.ChatConnection);
        }

        //get all connected users by chat
        public Task SendConnectedUsers(string chat)
        {
            var users = _connections.Values
                .Where(c => c.ChatConnection == chat)
                .Select(c => c.User);

            return Clients.Group(chat).SendAsync("UsersInChat", users);
        }

        //private List<Message> GetMessagesAsync()
        //{
        //    return await _context.Messages.ToListAsync();
        //    return MessageList;
        //}
        
    }
}
