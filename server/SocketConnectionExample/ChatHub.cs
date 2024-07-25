using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace SignalRExample
{
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();


        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            
            
        }
        public async Task AddUser(string user, string message)
        {
            if (!_connections.GetConnections(user).Contains(Context.ConnectionId))
            {
                _connections.Add(user, Context.ConnectionId);
            }
            await Clients.All.SendAsync("UserAdded", user, message);
        }

        //public Task JoinRoom(string roomName)
        //{
        //    return Groups.Add(Context.ConnectionId, roomName);
        //}

        //public Task LeaveRoom(string roomName)
        //{
        //    return Groups.Remove(Context.ConnectionId, roomName);
        //}
        public async Task AddUserToGroup(string group, string user, string message)
        {
            
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            //this group update works
            //seems that the group membership is not persisted beyond the current connection
            //will need to store connection ids for groups in a database
            await Clients.Group(group).SendAsync("GroupUpdate", "fdofaf");
            await Clients.All.SendAsync("UserAddedToGroup", group, user, message);
        }

        public async Task RemoveUserFromGroup(string group, string user, string message)
        {
            
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

            await Clients.All.SendAsync("UserRemovedFromGroup", group, user, message);
        }


        public void SendChatMessage(string who, string message)
        {

            foreach (var connectionId in _connections.GetConnections(who))
            {
                Clients.Client(connectionId).SendAsync("ReceiveMessage", who, message);
            }
        }

        public void SendChatMessageToGroup(string group, string message)
        {

            //Clients.Group(group).addChatMessage(name, message);
            Clients.Group(group).SendAsync("GroupUpdate", message);
        }

        public override Task OnConnectedAsync()
        {


            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {


            return base.OnDisconnectedAsync(exception);

        }



    }
}
