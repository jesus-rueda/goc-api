using System.Threading.Tasks;
using Goc.Business.Dtos;
using Goc.Models;

namespace Goc.Business.Contracts;

public interface ICharacterBl
{
    Task<Characters?> Get(int id);

    Task<TeamCharacterProfileDto?> GetProfile(string email);
}