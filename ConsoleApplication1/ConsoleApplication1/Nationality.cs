using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {

	public enum FACTION { AXIS, ALLIES, SPECIAL, NONE }


	public abstract class Nationality {

		protected char abbrev;
		protected string name;
		protected FACTION faction;

        protected Commander _player;

		protected Nationality() { }
        
		public char Abbrev {
			get { return abbrev; }
		}
		public FACTION Faction {
			get { return faction; }
		}
		public override string ToString() { 
			return name; 
		}
        
        public abstract Deck CreateNewDeck();
	}

	public sealed class American : Nationality {

        private static American instance;

		private American() : base() { 
			name = "American"; 
			abbrev = 'A';
			faction = FACTION.ALLIES;
        }
        public static American Instance
        {
            get
            {
                if (instance == null)
                    instance = new American();
                return instance;
            }
        }

        public override Deck CreateNewDeck() {
            return new AmericanDeck(American.Instance);
        }

        //TODO: finish adding leaders and units
		public class CplTwells : Leader {
			public CplTwells() : base(American.Instance, 6, 1, 1, 6, 1, 7, 0, 0, 1, 0, false, false, false, false, false, false) { }
		}
		public class PvtAdamson : Leader {
			public PvtAdamson() : base(American.Instance, 6, 2, 1, 6, 2, 7, 1, 0, 1, 0, false, false, false, false, false, false) { }
		}

		public class Engineer : Squad {
			public Engineer() : base(American.Instance, 8, 7, 3, 5, 8, 4, 1, 1, true, true, true, false, false, false) { }
		}
		public class Paratrooper : Squad {
			public Paratrooper() : base(American.Instance, 8, 6, 4, 5, 9, 4, 1, 1, true, true, true, false, false, false) { }
		}

		public class Elite : Team {
			public Elite() : base(American.Instance, 7, 4, 4, 5, 8, 2, 1, 1, true, false, true, false, false, false) { }
		}
		public class Weapon : Team {
			public Weapon() : base(American.Instance, 7, 2, 2, 4, 9, 1, 1, 1, false, false, false, false, false, false) { }
		}
	}

	public sealed class German : Nationality {
		private static German instance;
		private German() : base() { 
			name = "German";
			abbrev = 'G';
			faction = FACTION.AXIS;
		}
		public static German Instance {
			get {
				if (instance == null)
					instance = new German();
				return instance;
			}
		}

        public override Deck CreateNewDeck() {
            return new GermanDeck(German.Instance);
        }

		public class CplGuttman : Leader {
			public CplGuttman() : base(German.Instance, 6, 1, 1, 6, 1, 7, 0, 0, 1, 0, false, false, false, false, false, false) { }
		}
		public class CplWinkler : Leader {
			public CplWinkler() : base(German.Instance, 6, 1, 1, 6, 1, 7, 0, 0, 1, 0, false, false, false, false, false, false) { }
		}
		public class LtvKarsties : Leader {
			public LtvKarsties() : base(German.Instance, 9, 2, 1, 6, 2, 10, 1, 0, 1, 0, false, false, false, false, false, false) { }
		}
		public class PvtHerzog : Leader {
			public PvtHerzog() : base(German.Instance, 6, 2, 1, 6, 2, 7, 1, 0, 1, 0, false, false, false, false, false, false){ }
		}

		public class Pioneer : Squad {
			public Pioneer() : base(German.Instance, 8, 7, 3, 5, 9, 4, 1, 1, true, true, true, false, false, false) { }
		}
		public class Rifle : Squad {
			public Rifle() : base(German.Instance, 7, 5, 5, 4, 7, 2, 1, 1, false, true, true, false, false, false) { }
		}
		public class SS : Squad {
			public SS() : base(German.Instance, 8, 6, 5, 5, 10, 3, 1, 1, true, true, true, false, false, false) { }
		}

		public class Elite : Team {
			public Elite() : base(German.Instance, 8, 3, 3, 5, 7, 1, 1, 1, false, true, true, false, false, false) { }
		}
		public class Weapon : Team {
			public Weapon() : base(German.Instance, 8, 2, 2, 4, 9, 1, 1, 1, false, false, false, false, false, false) { }
		}

		public class LightMG : CombatCommander.Weapon {
			public LightMG() : base(German.Instance, 4, 8, 0, 5, 6, 7, 8, true, true) { }
		}

	}
	public sealed class Russian : Nationality {
		private static Russian instance;
		private Russian() : base() { 
			name = "Russian";
			abbrev = 'R';
			faction = FACTION.ALLIES;
		}
		public static Russian Instance {
			get {
				if (instance == null)
					instance = new Russian();
				return instance;
			}
		}
        
        public override Deck CreateNewDeck() {
            return new RussianDeck(Russian.Instance);
        }

		public class CplGordov : Leader {
			public CplGordov() : base(Russian.Instance, 6, 1, 1, 6, 1, 7, 0, 0, 1, 0, false, false, false, false, false, false) { }
		}
		public class CplKrylov : Leader {
			public CplKrylov() : base(Russian.Instance, 7, 1, 1, 6, 1, 8, 0, 0, 1, 0, false, false, false, false, false, false) { }
		}
		public class PvtGelon : Leader {
			public PvtGelon() : base(Russian.Instance, 6, 2, 1, 6, 2, 7, 1, 0, 1, 0, false, false, false, false, false, false) { }
		}
		public class SgtKovalev : Leader {
			public SgtKovalev() : base(Russian.Instance, 8, 2, 1, 6, 2, 9, 1, 0, 1, 0, false, false, false, false, false, false) { }
		}

		public class Assault : Squad {
			public Assault() : base(Russian.Instance, 9, 7, 2, 5, 8, 4, 1, 1, true, true, true, false, false, false) { }
		}
		public class GuardsRifle : Squad {
			public GuardsRifle() : base(Russian.Instance, 8, 5, 5, 5, 8, 2, 1, 1, false, false, false, false, false, false) { }
		}
		public class GuardsSMG : Squad {
			public GuardsSMG() : base(Russian.Instance, 8, 6, 3, 5, 8, 3, 1, 1, true, true, false, false, false, false) { }
		}
		public class Militia : Squad {
			public Militia() : base(Russian.Instance, 7, 5, 2, 3, 5, 2, 1, 1, false, false, false, false, false, false) { }
		}
		public class Rifle : Squad {
			public Rifle() : base(Russian.Instance, 8, 5, 3, 4, 6, 2, 1, 1, false, false, false, false, false, false) { }
		}

		public class Elite : Team {
			public Elite() : base(Russian.Instance, 8, 3, 2, 5, 7, 1, 1, 1, false, false, false, false, false, false) { }
		}
		public class Weapon : Team {
			public Weapon() : base(Russian.Instance, 9, 2, 1, 4, 8, 1, 1, 1, false, false, false, false, false, false) { }
		}

		public class LightMG : CombatCommander.Weapon {
			public LightMG() : base(Russian.Instance, 3, 6, 0, 1, 2, 7, 10, true, true) { }
		}
		public class MediumMG : CombatCommander.Weapon {
			public MediumMG() : base(Russian.Instance, 6, 10, -2, 1, 4, 7, 10, false, true) { }
		}



	}
}
