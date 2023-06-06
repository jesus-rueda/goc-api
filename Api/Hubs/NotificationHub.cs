using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Goc.Api.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("RecieveMessage", user, message);
        }
    }
}
