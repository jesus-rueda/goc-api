using System.Threading.Tasks;
using Goc.Models;

namespace Goc.Business.Contracts;

public interface ICampaignBl
{
    Task<Campaigns> GetActive();
}