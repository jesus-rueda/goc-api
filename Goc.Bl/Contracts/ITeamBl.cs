using Goc.Models;

namespace Goc.Business.Contracts
{
    public interface ITeamBl
    {
        Task<List<Teams>> GetAll();

        Task<Teams?> Get(int id);
    }
}
