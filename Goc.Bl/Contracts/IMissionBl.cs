﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Goc.Business.Dtos;

namespace Goc.Business.Contracts
{
    public interface IMissionBl
    {
        Task<List<MissionsDto>> GetAllAsync();

        Task<MissionsDto> GetAsync(int id);
    }
}
