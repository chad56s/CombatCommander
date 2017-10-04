using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander
{
    public static class GameRules
    {

        /*
         * GAME INFO
         */
        public static FACTION GetOpposingFaction(FACTION f) {
            FACTION oppose = FACTION.NONE;

            switch (f) {
                case FACTION.ALLIES:
                    oppose = FACTION.AXIS;
                    break;
                case FACTION.AXIS:
                    oppose = FACTION.ALLIES;
                    break;
            }
            return oppose;
        }



        /*
         * MOVEMENT RULES
         */
        public static bool CanMoveIntoHex(Game g, Map m, FACTION f) {
            bool canMove = true;

            return canMove;
        }

    }
}
