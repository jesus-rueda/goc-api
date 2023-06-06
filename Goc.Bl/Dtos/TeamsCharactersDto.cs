namespace Goc.Business.Dtos
{
    public partial class TeamsCharactersDto
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int TeamCharacterId { get; set; }
        public string Email { get; set; }
        public int IsLeader { get; set; }
    }
}