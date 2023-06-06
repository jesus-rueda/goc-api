using System;

namespace Goc.Business.Dtos;

public class MissionsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Story { get; set; }
    public string Instructions { get; set; }
    public int Coinks { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
}
