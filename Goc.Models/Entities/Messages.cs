using System;

namespace Goc.Models;

public partial class Messages
{
    public int Id { get; set; }
    public int SenderTeam { get; set; }
    public int RecipientTeam { get; set; }
    public string Message { get; set; }
    public DateTime DateTime { get; set; }

    public virtual Team RecipientTeamNavigation { get; set; }
    public virtual Team SenderTeamNavigation { get; set; }
}