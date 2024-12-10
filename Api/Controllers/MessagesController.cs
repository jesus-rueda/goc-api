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
    private readonly IMessageService myMessageService;

    public MessagesController(IMessageService messageService)
    {
        this.myMessageService = messageService;
    }

    [HttpGet]
    [Route("{teamId}/{date}")]
    public async Task<ActionResult<List<MessagesDto>>> Get(int teamId, DateTime date)
    {
        var messages = await this.myMessageService.GetAsync(teamId, date);

        return messages;
    }
}