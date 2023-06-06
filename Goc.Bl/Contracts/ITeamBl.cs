using Goc.Models;

namespace Goc.Business.Contracts
{
    public interface ITeamBl
    {
        Task<List<Teams>> GetAllAsync();

        Task<Teams> GetAsync(int id);
    }
}
