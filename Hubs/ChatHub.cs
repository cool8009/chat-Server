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
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");
                SendConnectedUsers(userConnection.Room);
            }
            return base.OnDisconnectedAsync(exception);
        }


        //get details about user, the message context,room and send
        public async Task SendMessage  (string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room)
                 .SendAsync("ReceiveMessage", userConnection.User, message);
                EntityEntry<Message> entityEntry = await _context.Messages.AddAsync(new Message { DateSent = DateTime.Now, Content = message, Id = new Guid() });
                var checker = _context.SaveChanges();
            }
        }


        //get details about user connection, the  room, enter the room
        public async Task JoinRoom(UserConnection userConnection)
        {
            
            //to notify connection to the room
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            
            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser,
                $"{userConnection.User} joined {userConnection.Room}");

            await SendConnectedUsers(userConnection.Room);
        }

        //get all connected users by room
        public Task SendConnectedUsers(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }

        //private List<Message> GetMessagesAsync()
        //{
        //    return await _context.Messages.ToListAsync();
        //    return MessageList;
        //}
        
    }
}
