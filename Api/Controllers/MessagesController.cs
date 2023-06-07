using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Business.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Goc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController
{
    private readonly IMessageBl _messageBl;

    public MessagesController(IMessageBl messageBl)
    {
        _messageBl = messageBl;
    }

    [HttpGet]
    [Route("{teamId}/{date}")]
    public async Task<ActionResult<List<MessagesDto>>> Get(int teamId, DateTime date)
    {
        var messages = await _messageBl.GetAsync(teamId, date);

        return messages;
    }
}