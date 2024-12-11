using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Business.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    public class Duel
    {
        public int MoneyAmount { get; set; }

        public string RoomName { get; set; }

        public string PlayerUnoId { get; set; }

        public string PlayerTwoId { get; set; }
    }

    public class DuelService
    {
        public List<Duel> duelList = new List<Duel>();
    }
    public class GameHub: Hub
    {
        private DuelService myDualService;

        public GameHub(DuelService myDualService)
        {
            this.myDualService = myDualService;
        }
        public async Task JoinRoom(string roomName, string user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            var duel = this.myDualService.duelList.Find(d => d.RoomName == roomName);
            if (duel != null)
            {
                duel.PlayerTwoId = user;
            }
            else
            {

                duel = new Duel() { MoneyAmount = 10, PlayerUnoId = user, RoomName = roomName };
                this.myDualService.duelList.Add(duel);
            }

            await Clients.Group(roomName).SendAsync("SystemMessage", duel);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("SystemMessage", $"{Context.ConnectionId} left room {roomName}");
        }

        public async Task SendChoice(string roomName, string user, string choice)
        {
            await Clients.Group(roomName).SendAsync("ReceiveChoice", user, choice);
        }

        public async Task SendMessage(string roomName, string user, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
        }
    }
}
