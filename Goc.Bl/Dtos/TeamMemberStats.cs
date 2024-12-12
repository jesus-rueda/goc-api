using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Business.Dtos
{
    public class TeamMemberStats
    {
        public int AttacksDone { get; set; }
        public int AttacksTotal { get; set; }
        public int  DefensesUsed { get; set; }
        public int DefensesTotal { get; set; }

        public int DuelsUsed { get; set; }

        public int? DuelsTotal { get; set; }
    }
}
