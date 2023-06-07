using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Goc.Business.Extensions;
using Goc.Business.Services;
using Goc.Models;
using Microsoft.EntityFrameworkCore;

namespace Goc.Business;

public class MessageBl : IMessageBl
{
    private readonly GocContext _context;
    private readonly INotificationSerive _notificacionService;

    public MessageBl(GocContext context, INotificationSerive notificacionService)
    {
        _context = context;
        _notificacionService = notificacionService;
    }

    public async Task<MessagesDto> CreateAsync(MessagesDto messageDto)
    {
        Messages message = messageDto.ToEntity();
        if (message == null)
        {
            throw new Exception("Argument message is not valid");
        }

        _context.Messages.Add(message);
        var rowsAffected = await _context.SaveChangesAsync();
        if (rowsAffected > 0)
        {
            await _notificacionService.Send(messageDto.RecipientTeam, messageDto);

            return message.ToDto();
        }

        throw new Exception("It was not possible to save the message");

    }

    public async Task<List<MessagesDto>> GetAsync(int teamId, DateTime date)
    {
        var messages = await _context.Messages.Where(m => m.RecipientTeam == teamId && m.DateTime >= date).ToListAsync();

        return messages.ToDto();
    }
}

