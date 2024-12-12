using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goc.Models.Entities
{
    public class DuelRoom
    {
        
        public int RoomId { get; set; }
        public long ActionLogId { get; set; }


        public ActionLog ActionLog { get; set; }

        public int Bet { get; set; }

        public int ChallengerId { get; set; }

        public int DefenderId { get; set; }

        public string GameId { get; set; }

        public int Rounds { get; set; }

        public string GameState { get; set; }

        public string CurrentTurn { get; set; }

        public string Result { get; set; }



    }
}
