using Goc.Models;

namespace Goc.Business.Contracts
{
    public interface IMissionBl
    {
        Task<List<Missions>> GetAllAsync();

        Task<Missions?> GetAsync(int id);
    }
}
