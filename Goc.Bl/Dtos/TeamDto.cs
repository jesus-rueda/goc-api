namespace Goc.Business.Dtos;

public partial class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Coinks { get; set; }

    public int AttacksDone { get; set; }

    public int AttacksRecieved { get; set; }

    public int AttacksTotal { get; set; }
    public int DefensesUsed { get; set; }

    public int DefensesTotal { get; set; }

    public TeamCharacterProfileDto[] Members { get; set; }
}

