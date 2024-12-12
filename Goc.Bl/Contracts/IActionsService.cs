using Goc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Business.Contracts
{
    public interface IActionsService
    {
        Task<GocActionResult> Attack(int campaignId, ICampaignProfile user, int targetTeamId);

        Task<GocActionResult> SetupDefence(int campaignId, ICampaignProfile user, byte[] evidence);

        Task<GocActionResult> AttackDefense(int campaignId, int attackId, ICampaignProfile user, byte[] evidence);

        Task Bonus(int campaignId, ICampaignProfile user, byte[] evidence);

        Task<DuelAction> Duel(int campaignId, ICampaignProfile user, int teamId, int duelGameId, int betCoinks);

        Task<GocActionResult> FinishMission(int campaignId, int missionId, ICampaignProfile user, byte[] fileBytes);

        Task<GocActionResult> FinishGame(int roomId, string gameState, PlayerGameResult result, ICampaignProfile user);

        Task<DuelAction> GetDuelTurnData(int roomId, ICampaignProfile user);

        Task<DuelAction> EndDuelTurn(int roomId, string gameState, ICampaignProfile user);
    }
}
