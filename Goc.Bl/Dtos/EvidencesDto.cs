namespace Goc.Business.Dtos;

public partial class EvidencesDto
{
    public long Id { get; set; }
    public long ActionLogId { get; set; }
    public int CharacterId { get; set; }
    public string Image { get; set; }
    public bool IsValid { get; set; }
}