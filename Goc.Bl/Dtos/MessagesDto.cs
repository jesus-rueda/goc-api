using System;

namespace Goc.Business.Dtos;

public partial class MessagesDto
{
    public int Id { get; set; }
    public int SenderTeam { get; set; }
    public int RecipientTeam { get; set; }
    public string Message { get; set; }
    public DateTime DateTime { get; set; }
}