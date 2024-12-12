using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Business.Hubs
{
    using Goc.Business.Contracts;
    using Goc.Models;
    using Microsoft.AspNetCore.SignalR;

    //public class Duel
    //{
    //    public int MoneyAmount { get; set; }

    //    public string RoomName { get; set; }

    //    public string PlayerUnoId { get; set; }

    //    public string PlayerTwoId { get; set; }
    //}

    //public class DuelService
    //{
    //    public List<Duel> duelList = new List<Duel>();
    //}
    public class GameHub: Hub
    {
        private readonly IActionsService myActionsService;
        private readonly IUserService myUserService;

        //private DuelService myDualService;

        public GameHub(IActionsService actionsService, IUserService userService)
        {
            this.myActionsService = actionsService;
            this.myUserService = userService;
        }

        public async Task JoinRoom(int roomName, string user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName.ToString());
            
            var profile = await this.myUserService.GetProfileByUpn(user);
            var duelAction = await this.myActionsService.GetDuelTurnData(roomName, profile);


            await this.Clients.Group(roomName.ToString()).SendAsync("userJoinRoom", new
                                                                                     {
                                                                                         MembershipId = profile.MembershipId, 
                                                                                         CharacterId = profile.CharacterId, 
                                                                                         Upn = profile.Upn,
                                                                                      });

            await this.Clients.Group(roomName.ToString()).SendAsync("NextTurnMessage", new { CurrentTurnMembershipId = duelAction.CurrentTurnMembershipId, duelAction.GameState });
            

            //var duel = this.myDualService.duelList.Find(d => d.RoomName == roomName);
            //if (duel != null)
            //{
            //    duel.PlayerTwoId = user;
            //}
            //else
            //{

            //    duel = new Duel() { MoneyAmount = 10, PlayerUnoId = user, RoomName = roomName };
            //    this.myDualService.duelList.Add(duel);
            //}

            //await Clients.Group(roomName.ToString()).SendAsync("SystemMessage", duel);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("SystemMessage", $"{Context.ConnectionId} left room {roomName}");
        }

        public async Task SendChoice(int roomName, int  membershipId, string choice)
        {
            var profile = await myUserService.GetProfileByMemberId(membershipId);
            var duelAction = await myActionsService.EndDuelTurn(roomName, choice, profile);

            await Clients.Group(roomName.ToString()).SendAsync("NextTurnMessage", new { CurrentTurnMembershipId = duelAction.CurrentTurnMembershipId, duelAction.GameState });
        }

        public async Task FinishDuel(int roomName, int memberId, PlayerGameResult result)
        {
            try
            {
                var profile = await myUserService.GetProfileByMemberId(memberId);
                var game = await myActionsService.FinishGame(roomName, null, result, profile);

                await Clients.Group(roomName.ToString()).SendAsync("GameFinished", memberId, game.WinnerMemberId );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }
    }
}
