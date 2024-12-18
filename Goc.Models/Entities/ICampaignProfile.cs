﻿// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Models;

public interface ICampaignProfile
{
    public int? CampaignId { get; set; }

    public int? MembershipId { get; set; }
        
    public int Id { get; }
        
    public int? TeamId { get;  }
        
    public int? CharacterId { get;  }
        
    public string Upn { get;  }

    public bool IsLeader { get; }

    public bool IsAdmin { get;  }
}