using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;

namespace Goc.Business;

public class CharacterBl : ICharacterBl
{
    private readonly GocContext _context;

    public CharacterBl(GocContext context)
    {
        _context = context;
    }

    public async Task<Characters?> Get(int id)
    {
        var character = await _context.Characters.FindAsync(id);
        return character;
    }
}