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

        Task<Duel> Duel(int campaignId, ICampaignProfile user, int teamId, int duelGameId, int betCoinks);

        Task<GocActionResult> FinishMission(int campaignId, int missionId, ICampaignProfile user, byte[] fileBytes);
    }
}
