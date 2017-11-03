using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {
    public partial class GameManager {

        public bool MoveFrom(Unit u) {
           /* Hex oldLocation = u.location;
            oldLocation.UpdateObjective();
            u.location = null;
            return stack.Remove(u);*/
            return false;
        }

        public bool MoveInto(Unit u) {
           /* bool success = false;

            if (u.location != null)
                u.location.MoveFrom(u);

            u.location = this;
            success = stack.Add(u);
            u.location.UpdateObjective();
            return success;
            */
            return false;
        }

    }
}
