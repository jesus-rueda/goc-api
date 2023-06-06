using System.Threading.Tasks;
using Goc.Models;

namespace Goc.Business.Contracts;

public interface ICharacterBl
{
    Task<Characters?> Get(int id);
}