using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {
    public partial class GameManager {

        private Map.ObjectiveHexes getMapObjective(int o) {
            if (o < 1 || o > 5)
                throw new ArgumentOutOfRangeException("o", "invalid objective number");
            return _gameboard.map.GetObjective(o);
        }

    }
}
