﻿using System;

namespace Goc.Models;

public partial class Messages
{
    public int Id { get; set; }
    public int SenderTeam { get; set; }
    public int RecipientTeam { get; set; }
    public string Message { get; set; }
    public DateTime DateTime { get; set; }

    public virtual Teams RecipientTeamNavigation { get; set; }
    public virtual Teams SenderTeamNavigation { get; set; }
}