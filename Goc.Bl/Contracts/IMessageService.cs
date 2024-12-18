﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Dtos;

namespace Goc.Business.Contracts
{
    public interface IMessageService
    {
        Task<List<MessagesDto>> GetAsync(int teamId, DateTime date);

        Task<MessagesDto> SendAsync(MessagesDto message);
    }
}
