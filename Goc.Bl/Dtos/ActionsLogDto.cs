using System;

namespace Goc.Business.Dtos;

public partial class ActionsLogDto
{
    public long Id { get; set; }
    public int MissionId { get; set; }
    public int TeamId { get; set; }
    public int ActionTypeId { get; set; }
    public int CharacterId { get; set; }
    public int? AffectedTeamId { get; set; }
    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo { get; set; }
    public int Coinks { get; set; }
}