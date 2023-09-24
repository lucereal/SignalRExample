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


        public void SendChatMessage(string who, string message)
        {

            foreach (var connectionId in _connections.GetConnections(who))
            {
                Clients.Client(connectionId).SendAsync("ReceiveMessage", who, message);
            }
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
