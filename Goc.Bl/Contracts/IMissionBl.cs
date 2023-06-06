using Goc.Models;

namespace Goc.Business.Contracts
{
    public interface IMissionBl
    {
        Task<List<Missions>> GetAll();

        Task<Missions?> Get(int id);
    }
}
