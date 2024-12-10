using System;

namespace Goc.Models;

using System.ComponentModel.DataAnnotations.Schema;

public partial class Message
{
    public int Id { get; set; }
    public int SenderTeam { get; set; }
    public int RecipientTeam { get; set; }

    [Column("Message")]
    public string Text { get; set; }
    public DateTime DateTime { get; set; }

    public virtual Team RecipientTeamNavigation { get; set; }
    public virtual Team SenderTeamNavigation { get; set; }
}