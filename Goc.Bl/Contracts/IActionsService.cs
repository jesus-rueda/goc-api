using Goc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Business.Contracts
{
    public interface IActionsService
    {
        Task<ActionResult> Attack(int campaignId, ICampaignProfile user, int targetTeamId);

        Task<ActionResult> SetupDefence(int campaignId, ICampaignProfile user, byte[] evidence);

        Task<ActionResult> AttackDefense(int campaignId, int attackId, ICampaignProfile user, byte[] evidence);

        Task Bonus(int campaignId, ICampaignProfile user, byte[] evidence);

        Task<Duel> Duel(int campaignId, ICampaignProfile user, int duelTargetTeamId, int duelGameId, int duelBetCoinks);

        Task<ActionResult> FinishMission(int campaignId, int missionId, ICampaignProfile user, byte[] fileBytes);
    }
}
