using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Models;

namespace Goc.Business.Contracts;

public interface ICharacterBl
{
    Task<Character?> Get(int id);

    Task<IEnumerable<Character>> GetAll();
    byte[] GetImage(int characterId, string type);

    //Task<TeamCharacterProfileDto?> GetProfile(string email);
}