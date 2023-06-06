using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Api.Dtos;

namespace Goc.Business.Contracts
{
    public interface IMessageBl
    {
        Task<List<MessagesDto>> GetAsync(int teamId, DateTime date);
    }
}
