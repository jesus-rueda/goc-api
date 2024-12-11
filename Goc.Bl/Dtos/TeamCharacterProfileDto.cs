namespace Goc.Business.Dtos;

public class TeamCharacterProfileDto
{

    public int MembershipId { get; set; }
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public int CharacterId { get; set; }
    public string CharacterName { get; set; }
    public string Upn { get; set; }
    public int IsLeader { get; set; }

    public bool IsPendingAproval { get; set; }
}