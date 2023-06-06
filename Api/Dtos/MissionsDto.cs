namespace Goc.Api.Dtos;

public partial class MissionsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Story { get; set; }
    public string Instructions { get; set; }
    public int Coinks { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}