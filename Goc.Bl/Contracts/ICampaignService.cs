using System.Threading.Tasks;
using Goc.Models;

namespace Goc.Business.Contracts;

public interface ICampaignService
{
    Task<Campaign> GetActive();
}