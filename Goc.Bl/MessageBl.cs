using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

public class MessageBl : IMessageBl
{
    private readonly GocContext _context;

    public MessageBl(GocContext context)
    {
        _context = context;
    }

    public async Task<List<MessagesDto>> GetAsync(int teamId, DateTime date)
    {
        var messages = await _context.Messages.Where(m => m.RecipientTeam == teamId && m.DateTime >= date).ToListAsync();

        return messages.ToDto();
    }
}

