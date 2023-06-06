using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Models;

namespace Goc.Business.Contracts;

public interface ICampaignBl
{
    Task<List<Campaigns>> GetAll();
}