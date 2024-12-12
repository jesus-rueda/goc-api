// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Business;

using System;
using System.Linq;
using System.Threading.Tasks;
using Goc.Api.Dtos;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Models;
using Goc.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

internal class ActionsService : IActionsService
{
    #region Fields

    private readonly GocContext myContext;

    private readonly IEvidenceService myEvidenceService;

    private readonly IMessageService myMessageService;

    private readonly ITeamService myTeamService;

    private readonly IMissionService myMissionService;

    private readonly ICampaignService myCampaignService;

    #endregion

    #region Constructors

    public ActionsService(
        IEvidenceService evidenceService,
        ITeamService teamService,
        IMessageService messageService,
        ICampaignService campaignService,
        IMissionService missionService,
        GocContext context)
    {
        this.myEvidenceService = evidenceService;
        this.myTeamService = teamService;
        this.myContext = context;
        this.myMessageService = messageService;
        this.myMissionService = missionService;
        this.myCampaignService = campaignService;
    }

    #endregion

    #region Methods

    private Task<bool> HaveSetupDefence(int campaignId, int teamId)
    {
        return this.myContext.ActionsLog.Include(x => x.TeamCharacter)
            .AnyAsync(
                      x => x.TeamCharacter.CampaignId == campaignId // this campaign
                           && x.TeamCharacter.TeamId == teamId && // target team
                           x.DateTimeFrom <= DateTime.Now && DateTime.Now < x.DateTimeTo && // active
                           x.ActionTypeId == (int)ActionType.SetupDefence);
    }

    public async Task<GocActionResult> Attack(int campaignId, ICampaignProfile user, int targetTeamId)
    {
        var attackerTeam = await this.myTeamService.GetAsync(campaignId, user.TeamId!.Value);
        var teamId = user.TeamId!.Value;
        var affectedTeam = await this.myTeamService.GetAsync(campaignId, targetTeamId);
        if (affectedTeam == null)
        {
            throw new Exception("Affected team not found");
        }

        // available attacks ?
        var parms = await this.myCampaignService.GetParametersFor(campaignId, ActionType.Attack);

        var attacks = await this.myContext.ActionsLog.Include(x => x.TeamCharacter)
            .CountAsync(
                        x => x.TeamCharacter.CampaignId == campaignId // this campaign
                             && x.TeamCharacterId == user.MembershipId && // target team
                             x.ActionTypeId == (int)ActionType.Attack);

        if (attacks >= parms.MaxAllowed)
        {
            return new GocActionResult() { Effective = false, Message = "No more attacks available for user" };
        }

        // have setup a prev defence ?
        var isAffectedTeamDefended = await this.HaveSetupDefence(campaignId, teamId);

        // send messages
        if (isAffectedTeamDefended)
        {
            var defTemplate = await this.myContext.MessageTemplates.FirstOrDefaultAsync(mt => mt.ActionTypeId == (int)ActionType.SetupDefence);
            await this.myMessageService.SendAsync(
                                                  new MessagesDto
                                                  {
                                                      DateTime = DateTime.UtcNow,
                                                      Message = defTemplate.Body.Replace("<attackedteam>", affectedTeam.Name),
                                                      RecipientTeam = user.TeamId!.Value,
                                                      SenderTeam = affectedTeam.Id
                                                  });
        }
        else
        {
            var characterSkills = await this.myContext.Characters.FirstOrDefaultAsync(c => c.Id == user.CharacterId);
            var atkTemplate = await this.myContext.MessageTemplates.FirstOrDefaultAsync(mt => mt.ActionTypeId == (int)ActionType.Attack);

            await this.myMessageService.SendAsync(
                                                  new MessagesDto
                                                  {
                                                      DateTime = DateTime.UtcNow,
                                                      Message = atkTemplate.Body.Replace("<attacker>", attackerTeam.Name)
                                                          .Replace("<attackdescription>", characterSkills.Attack),
                                                      RecipientTeam = affectedTeam.Id,
                                                      SenderTeam = teamId
                                                  });
        }

        var result = new GocActionResult()
                     {
                         Effective = !isAffectedTeamDefended, Message = "Attack was successfully", Coinks = isAffectedTeamDefended ? 0 : parms.Coinks
                     };

        await this.myEvidenceService.RegisterAsync(campaignId, ActionType.Attack, user.MembershipId!.Value, result.Coinks, parms.Duration, targetTeamId);
        return result;
    }

    public async Task<GocActionResult> SetupDefence(int campaignId, ICampaignProfile user, byte[] evidence)
    {
        var parms = await this.myCampaignService.GetParametersFor(campaignId, ActionType.Attack);

        // still have defense available in campaign ?
        var defencesUsed = await this.myContext.ActionsLog.Include(x => x.TeamCharacter)
            .CountAsync(
                        x => x.TeamCharacter.CampaignId == campaignId // this campaign
                             && x.TeamCharacterId == user.MembershipId && x.ActionTypeId == (int)ActionType.SetupDefence);

        if (defencesUsed >= parms.MaxAllowed)
        {
            return new GocActionResult() { Message = "No more defences available for user", Coinks = 0, Effective = false };
        }

        // already have defense ?
        var isAffectedTeamDefended = await this.HaveSetupDefence(campaignId, user.TeamId!.Value);
        if (isAffectedTeamDefended)
        {
            return new GocActionResult() { Effective = false, Message = "Team already have a defence prepared" };
        }

        // all ok, setup defence
        await this.myEvidenceService.RegisterAsync(campaignId, ActionType.SetupDefence, user.MembershipId!.Value, parms.Coinks, parms.Duration);
        return new GocActionResult() { Effective = true, Message = "AttackDefense prepared" };
    }

    public Task<GocActionResult> AttackDefense(int campaignId, int attackId, ICampaignProfile user, byte[] evidence)
    {
        throw new NotImplementedException();
    }

    public Task Bonus(int campaignId, ICampaignProfile user, byte[] evidence)
    {
        throw new NotImplementedException();
    }



    public async Task<GocActionResult> FinishGame(int roomId, string gameState, PlayerGameResult result, ICampaignProfile user)
    {
        var campaignId = user.CampaignId!.Value;
        var parms = await this.myCampaignService.GetParametersFor(campaignId, ActionType.DuelChallenge);
        var room = await this.myContext.DuelRooms.FindAsync(roomId);
        if (room == null)
        {
            return new GocActionResult() { Effective = false, Message = "Room not found" };
        }

        room.GameState = gameState;
        room.Result = result.ToString();
        room.CurrentTurn = null;

        var challenger = await this.myContext.Memberships.FindAsync(room.ChallengerId);
        var defender = await this.myContext.Memberships.FindAsync(room.DefenderId);

        var imChallenger = challenger.MembershipId == user.MembershipId;

        var winner = challenger;
        var loser = defender;
        var coinks = room.Bet;

        switch (result)
        {
            case PlayerGameResult.Lose:

                break;
            case PlayerGameResult.Win:
                //(winner, loser) = (loser, winner);
                break;
            case PlayerGameResult.Draw:
                winner = null;
                loser = null;
                coinks = 0;
                break;
        }

        // register the duel end and coinks balance
        await this.myEvidenceService.RegisterAsync(campaignId,
                                                   ActionType.DuelEnd,
                                                   winner?.MembershipId ?? user.MembershipId!.Value,
                                                   coinks,
                                                   parms.Duration,
                                                   affectedTeamId: loser?.MembershipId);

        await this.myContext.SaveChangesAsync();
        return new GocActionResult() { Coinks = room.Bet, Effective = true, Message = "Game completed" };
    }

    public async Task<DuelAction> GetDuelTurnData(int roomId, ICampaignProfile user)
    {   
        var  room =  await this.myContext.DuelRooms
            .Include(x=>x.ActionLog)
            .Where(x => x.RoomId == roomId)
            .FirstOrDefaultAsync();


        var challenger = await this.myContext.Memberships.FindAsync(room.ChallengerId);
        var defender = await this.myContext.Memberships.FindAsync(room.DefenderId);

        
        if (room.ChallengerId != user.MembershipId && room.DefenderId != user.MembershipId)
        {
            return new DuelAction() { Effective = false, Message = "User not in the room" };
        }



        var isMyTurn = room.CurrentTurn == PlayerType.Challenger.ToString() && room.ChallengerId == user.MembershipId
                      || room.CurrentTurn == PlayerType.Defender.ToString() && room.DefenderId == user.MembershipId;


        var duel = new DuelAction()
                         {
                             Id = room.RoomId,
                             Coinks = room.Bet,
                             GameState = room.GameState,
                             GameId = room.GameId,
                             Deadline = room.ActionLog.DateTimeTo,
                             Effective = true,
                         };

            return duel;
    }

    public async Task<DuelAction> EndDuelTurn(int roomId, string gameState, ICampaignProfile user)
    {
        var room = await this.myContext.DuelRooms
            .Include(x => x.ActionLog)
            .Where(x => x.RoomId == roomId)
            .FirstOrDefaultAsync();

        if (room.ChallengerId != user.MembershipId && room.DefenderId != user.MembershipId)
        {
            return new DuelAction() { Effective = false, Message = "User not in the room" };
        }

        var isMyTurn = room.CurrentTurn == PlayerType.Challenger.ToString() && room.ChallengerId == user.MembershipId
                       || room.CurrentTurn == PlayerType.Defender.ToString() && room.DefenderId == user.MembershipId;


        if (!isMyTurn)
        {
            return new DuelAction() { Effective = false, Message = "Not you turn" };
        }

        room.GameState = gameState;
        room.CurrentTurn = room.CurrentTurn == PlayerType.Challenger.ToString() ? PlayerType.Defender.ToString() : PlayerType.Challenger.ToString();
        this.myContext.DuelRooms.Update(room);
        await this.myContext.SaveChangesAsync();

        var duel = new DuelAction()
                   {
                       Id = room.RoomId,
                       Coinks = room.Bet,
                       GameState = room.GameState,
                       GameId = room.GameId,
                       IsMyTurn = false,
                       Deadline = room.ActionLog.DateTimeTo,
                       Effective = true,
                   };

        return duel;
    }

    public async Task<DuelAction> Duel(int campaignId, ICampaignProfile user, int teamId, int duelGameId, int betCoinks)
    {
        var parms = await this.myCampaignService.GetParametersFor(campaignId, ActionType.DuelChallenge);
        var logId = await this.myEvidenceService.RegisterAsync(campaignId, ActionType.DuelChallenge, user.MembershipId!.Value, 0, parms.Duration);

        var defenderTeam = await this.myTeamService.GetAsync(campaignId, teamId);
        var challengerTeam = await this.myTeamService.GetAsync(campaignId, user.TeamId!.Value);

        var random = new Random();
        var defender = defenderTeam.Members.ElementAt(random.Next(defenderTeam.Members.Count()));

        betCoinks = Math.Min(betCoinks, challengerTeam.Coinks);
        betCoinks = Math.Min(betCoinks, defenderTeam.Coinks);


        if(betCoinks < parms.Coinks)
        {
            return new DuelAction() { Effective = false, Message = $"Min bet can be {parms.Coinks}" };
        }

        if (betCoinks > parms.MaxAllowed)
        {
            return new DuelAction() { Effective = false, Message = $"Max bet can be {parms.MaxAllowed}" };
        }

        var room = new DuelRoom()
                   {
                       ActionLogId = logId,
                       Bet = betCoinks,
                       GameId = duelGameId.ToString(),
                       ChallengerId = user.MembershipId!.Value,
                       DefenderId = defender.MembershipId,
                       CurrentTurn = PlayerType.Challenger.ToString(),
                       Rounds = 3
                   };

        this.myContext.DuelRooms.Add(room);
        await this.myContext.SaveChangesAsync();

        var duel = new DuelAction
                   {
                       Coinks = betCoinks,
                       Deadline = DateTime.Now.Add(parms.Duration),
                       Effective = true,
                       Id = room.RoomId,
                       Oponent = defender
                   };

        return duel;
    }

    public async Task<GocActionResult> FinishMission(int campaignId, int missionId, ICampaignProfile user, byte[] fileBytes)
    {
        // still open?
        var mission = await this.myMissionService.GetAsync(campaignId, missionId);
        if (mission.StartDate > DateTime.Now || DateTime.Now > mission.EndDate)
        {
            return new GocActionResult() { Effective = false, Message = "Mission is not active" };
        }

        var actions = await this.myContext.ActionsLog.Include(x => x.TeamCharacter)
            .Include(x => x.Evidences)
            .Where(
                   x => x.TeamCharacter.TeamId == user.TeamId && x.MissionId == missionId && x.ActionTypeId == (int)ActionType.CompleteMission
                        && x.Evidences.Any(e => e.IsValid))
            .Select(x => new { UserId = x.TeamCharacter.UserId })
            .ToListAsync();

        // im not already finished this mission ?
        if (actions.Any(x => x.UserId == user.Id))
        {
            return new GocActionResult() { Effective = false, Message = "Mission already complete for this user" };
        }

        var membersCount = await this.myContext.Memberships.Where(x => x.TeamId == user.TeamId && x.CampaignId == campaignId)
            .CountAsync();

        var coinks = 0;
        if (actions.Count + 1 == membersCount)
        {
            // all members finished the mission, last one increments coinks
            coinks = mission.Coinks;
        }

        var parms = await this.myCampaignService.GetParametersFor(campaignId, ActionType.CompleteMission);

        await this.myEvidenceService.RegisterAsync(
                                                   campaignId,
                                                   ActionType.CompleteMission,
                                                   user.MembershipId!.Value,
                                                   coinks,
                                                   parms.Duration,
                                                   missionId: missionId,
                                                   evidence: fileBytes);
        return new GocActionResult() { Effective = true, Coinks = coinks, Message = "Mission completed" };
    }

    #endregion
}