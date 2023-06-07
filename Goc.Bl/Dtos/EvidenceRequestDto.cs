using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Business.Dtos
{
    public class EvidenceRequestDto
    {
        public int MissionId { get; set; }
        public int TeamId { get; set; }
        public int ActionId { get; set; }
        public int TeamCharacterId { get; set; }
        public int AffectedTeamId { get; set; }
        public string Image { get; set; }
    }
}
