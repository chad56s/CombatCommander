using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {
    public partial class GameManager {

        public static class Rules {

            public static int HandSize(POSTURE p) {
                int handsize = 0;
                switch (p) {
                    case POSTURE.ATTACK:
                        handsize = 6;
                        break;
                    case POSTURE.DEFEND:
                        handsize = 4;
                        break;
                    case POSTURE.RECON:
                        handsize = 5;
                        break;
                }
                return handsize;
            }

        }
        
    }
}
