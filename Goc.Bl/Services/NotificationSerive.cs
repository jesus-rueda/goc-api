using System.Threading.Tasks;
using Goc.Business.Dtos;
using Goc.Business.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Goc.Business.Services
{
    public interface INotificationSerive
    {
        Task Send(int teamId, MessagesDto message);
    }
    public class NotificationSerive : INotificationSerive
    {
        private readonly IHubContext<NotificationHub> notificationContext;

        public NotificationSerive(IHubContext<NotificationHub> notificationContext)
        {
            this.notificationContext = notificationContext;
        }

        public async Task Send(int teamId, MessagesDto message)
        {
            var messageString = JsonConvert.SerializeObject(message);

            await notificationContext.Clients.All.SendAsync("RecieveMessage", teamId, messageString);
        }
    }
}
