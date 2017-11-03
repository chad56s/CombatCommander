using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {

    public class ObjectiveChit_PW {
        /* Player wrapper for ObjectiveChit */
        private ObjectiveChit _oc;
        private FACTION _f;

        public ObjectiveChit_PW(FACTION f, ObjectiveChit oc) {
            _oc = oc;
            _f = f;
        }

        public bool IsSecret {
            get { return _oc.IsSecret; }
        }

        public char Letter {
            get { return _oc.Letter; }
        }

        public int Numbers {
            get { return _oc.Numbers; }
        }

        public int Value {
            get { return _oc.Value; }
        }

        public bool AppliesToObjectiveNumber(int n) { return _oc.AppliesToObjectiveNumber(n); }

    }

    public class ScenarioInfo_PW {

        private Scenario _s;

        public ScenarioInfo_PW(Scenario s) {
            _s = s;
        }
    }

}