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
        public abstract CounterMix CreateNewCounterMix();
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

        public override CounterMix CreateNewCounterMix() {
            return new AmericanCounterMix(American.Instance);
        }

        /*
         * American Leaders
         */
        public class Hero : CombatCommander.Hero {
            public Hero() : base(American.Instance, "Lucas", 9, 2, 4, 7, 0, 9, 1, 2, 3, 0, false, false, false, false, false, false) { }
        }
		public class Private : Leader {
			public Private(string name) : base(American.Instance, name, 6, 2, 1, 6, 2, 7, 1, 0, 1, 0, false, false, false, false, false, false) { }
		}
        public class CorporalX : Leader {
            public CorporalX(string name) : base(American.Instance, name, 6, 1, 1, 6, 1, 7, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
		public class CorporalY : Leader {
			public CorporalY(string name) : base(American.Instance, name, 7, 1, 1, 6, 1, 8, 0, 0, 1, 0, false, false, false, false, false, false) { }
		}
        public class SergeantX : Leader {
            public SergeantX(string name) : base(American.Instance, name, 8, 1, 1, 6, 1, 9, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class SergeantY : Leader {
            public SergeantY(string name) : base(American.Instance, name, 8, 2, 1, 6, 2, 9, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class LieutenantX : Leader {
            public LieutenantX(string name) : base(American.Instance, name, 9, 1, 1, 6, 1, 10, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class LieutenantY : Leader {
            public LieutenantY(string name) : base(American.Instance, name, 9, 2, 1, 6, 2, 10, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class Captain : Leader {
            public Captain(string name) : base(American.Instance, name, 10, 2, 1, 6, 2, 11, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }

        /*
         * American Squads
         */
		public class Engineer : Squad {
			public Engineer() : base(American.Instance, "Engineer", 8, 7, 3, 5, 8, 4, 1, 1, true, true, true, false, false, false) { }
		}
		public class Paratroop : Squad {
			public Paratroop() : base(American.Instance, "Paratroop", 8, 6, 4, 5, 9, 4, 1, 1, true, true, true, false, false, false) { }
		}
        public class EliteSquad : Squad {
            public EliteSquad() : base(American.Instance, "Elite", 7, 6, 6, 5, 8, 3, 1, 1, true, false, true, false, false, false) { }
        }
        public class LineSquad : Squad {
            public LineSquad() : base(American.Instance, "Line", 6, 6, 6, 4, 8, 3, 1, 1, true, false, true, false, false, false) { }
        }
        public class GreenSquad : Squad {
            public GreenSquad() : base(American.Instance, "Green", 5, 5, 4, 3, 7, 3, 1, 1, true, false, true, false, false, false) { }
        }

        /*
         * American Teams
         */
        public class Weapon : Team {
            public Weapon() : base(American.Instance, "Weapon", 7, 2, 2, 4, 9, 1, 1, 1, false, false, false, false, false, false) { }
        }
		public class Elite : Team {
			public Elite() : base(American.Instance, "Elite", 7, 4, 4, 5, 8, 2, 1, 1, true, false, true, false, false, false) { }
		}
        public class Line : Team {
            public Line() : base(American.Instance, "Line", 6, 3, 4, 4, 7, 2, 1, 1, true, false, true, false, false, false) { }
        }
        public class Green : Team {
            public Green() : base(American.Instance, "Green", 5, 3, 3, 3, 6, 2, 1, 1, true, false, true, false, false, false) { }
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

        public override CounterMix CreateNewCounterMix() {
            return new GermanCounterMix(German.Instance);
        }

        /*
         * German Leaders
         */
        public class Hero : CombatCommander.Hero {
            public Hero() : base(German.Instance, "German Hero", 9, 2, 4, 7, 0, 9, 1, 2, 3, 0, false, false, false, false, false, false) { }
        }
        public class Private : Leader {
            public Private(string name) : base(German.Instance, name, 6, 2, 1, 6, 2, 7, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class CorporalX : Leader {
            public CorporalX(string name) : base(German.Instance, name, 6, 1, 1, 6, 1, 7, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class CorporalY : Leader {
            public CorporalY(string name) : base(German.Instance, name, 7, 1, 1, 6, 1, 8, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class SergeantX : Leader {
            public SergeantX(string name) : base(German.Instance, name, 8, 1, 1, 6, 1, 9, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class SergeantY : Leader {
            public SergeantY(string name) : base(German.Instance, name, 8, 2, 1, 6, 2, 9, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class LieutenantX : Leader {
            public LieutenantX(string name) : base(German.Instance, name, 9, 1, 1, 6, 1, 10, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class LieutenantY : Leader {
            public LieutenantY(string name) : base(German.Instance, name, 9, 2, 1, 6, 2, 10, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class Captain : Leader {
            public Captain(string name) : base(German.Instance, name, 10, 2, 1, 6, 2, 11, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }

        /*
         * German Squads
         */
		public class Pionier : Squad {
			public Pionier() : base(German.Instance, "Pionier", 8, 7, 3, 5, 9, 4, 1, 1, true, true, true, false, false, false) { }
		}
        public class SS : Squad {
            public SS() : base(German.Instance, "SS", 8, 6, 5, 5, 10, 3, 1, 1, true, true, true, false, false, false) { }
        }
        public class EliteRifle : Squad {
            public EliteRifle() : base(German.Instance, "Elite Rifle", 8, 5, 6, 5, 8, 2, 1, 1, false, true, true, false, false, false) { }
        }
        public class Parachute : Squad {
            public Parachute() : base(German.Instance, "Parachute", 8, 5, 4, 5, 8, 3, 1, 1, true, true, true, false, false, false) { }
        }
		public class Rifle : Squad {
			public Rifle() : base(German.Instance, "Rifle", 7, 5, 5, 4, 7, 2, 1, 1, false, true, true, false, false, false) { }
		}
        public class Volksgrenadier : Squad {
            public Volksgrenadier() : base(German.Instance, "Volksgrenadier", 7, 5, 4, 4, 6, 2, 1, 1, false, true, false, false, false, false) { }
        }
        public class Conscript : Squad {
            public Conscript() : base(German.Instance, "Conscript", 6, 5, 3, 3, 5, 2, 1, 1, false, true, false, false, false, false) { }
        }

        /*
        * German Teams
        */
        public class Weapon : Team {
            public Weapon() : base(German.Instance, "Weapon", 8, 2, 2, 4, 9, 1, 1, 1, false, false, false, false, false, false) { }
        }
		public class Elite : Team {
			public Elite() : base(German.Instance, "Elite", 8, 3, 3, 5, 7, 1, 1, 1, false, true, true, false, false, false) { }
		}
        public class Line : Team {
            public Line() : base(German.Instance, "Line", 7, 3, 3, 4, 6, 1, 1, 1, false, true, false, false, false, false) { }
        }
        public class Green : Team {
            public Green() : base(German.Instance, "Green", 6, 3, 3, 3, 4, 1, 1, 1, false, true, false, false, false, false) { }
        }

        /*
         * German Weapons
         */
		public class LightMG : CombatCommander.Weapon {
			public LightMG() : base(German.Instance, "Light MG", 4, 8, 0, 5, 6, 7, 8, true, true) { }
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

        public override CounterMix CreateNewCounterMix() {
            return new RussianCounterMix(Russian.Instance);
        }

        /*
         * Russian Leaders
         */
        public class Hero : CombatCommander.Hero {
            public Hero() : base(Russian.Instance, "Russian Hero", 9, 2, 4, 7, 0, 9, 1, 2, 3, 0, false, false, false, false, false, false) { }
        }
        public class Private : Leader {
            public Private(string name) : base(Russian.Instance, name, 6, 2, 1, 6, 2, 7, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class CorporalX : Leader {
            public CorporalX(string name) : base(Russian.Instance, name, 6, 1, 1, 6, 1, 7, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class CorporalY : Leader {
            public CorporalY(string name) : base(Russian.Instance, name, 7, 1, 1, 6, 1, 8, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class SergeantX : Leader {
            public SergeantX(string name) : base(Russian.Instance, name, 8, 1, 1, 6, 1, 9, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class SergeantY : Leader {
            public SergeantY(string name) : base(Russian.Instance, name, 8, 2, 1, 6, 2, 9, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class LieutenantX : Leader {
            public LieutenantX(string name) : base(Russian.Instance, name, 9, 1, 1, 6, 1, 10, 0, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class LieutenantY : Leader {
            public LieutenantY(string name) : base(Russian.Instance, name, 9, 2, 1, 6, 2, 10, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }
        public class Captain : Leader {
            public Captain(string name) : base(Russian.Instance, name, 10, 2, 1, 6, 2, 11, 1, 0, 1, 0, false, false, false, false, false, false) { }
        }

        /*
         * Russian Squads
         */
		public class Assault : Squad {
			public Assault() : base(Russian.Instance, "Assault", 9, 7, 2, 5, 8, 4, 1, 1, true, true, true, false, false, false) { }
		}
        public class GuardsSMG : Squad {
            public GuardsSMG() : base(Russian.Instance, "Guards SMG", 8, 6, 3, 5, 8, 3, 1, 1, true, true, false, false, false, false) { }
        }
		public class GuardsRifle : Squad {
			public GuardsRifle() : base(Russian.Instance, "Guards Rifle", 8, 5, 5, 5, 8, 2, 1, 1, false, false, false, false, false, false) { }
		}
        public class SMG : Squad {
            public SMG() : base(Russian.Instance, "SMG", 8, 5, 2, 4, 6, 3, 1, 1, true, true, false, false, false, false) { }
        }
        public class Rifle : Squad {
            public Rifle() : base(Russian.Instance, "Rifle", 8, 5, 3, 4, 6, 2, 1, 1, false, false, false, false, false, false) { }
        }
		public class Militia : Squad {
			public Militia() : base(Russian.Instance, "Militia", 7, 5, 2, 3, 5, 2, 1, 1, false, false, false, false, false, false) { }
		}

        /*
         * Russian Teams
         */
        public class Weapon : Team {
            public Weapon() : base(Russian.Instance, "Weapon", 9, 2, 1, 4, 8, 1, 1, 1, false, false, false, false, false, false) { }
        }
		public class Elite : Team {
			public Elite() : base(Russian.Instance, "Elite", 8, 3, 2, 5, 7, 1, 1, 1, false, false, false, false, false, false) { }
		}
        public class Line : Team {
            public Line() : base(Russian.Instance, "Line", 7, 3, 2, 4, 6, 1, 1, 1, false, false, false, false, false, false) { }
        }
        public class Green : Team {
            public Green() : base(Russian.Instance, "Green", 6, 3, 2, 3, 5, 1, 1, 1, false, false, false, false, false, false) { }
        }

        /*
         * Russian Weapons
         */
		public class LightMG : CombatCommander.Weapon {
			public LightMG() : base(Russian.Instance, 3, 6, 0, 1, 2, 7, 10, true, true) { }
		}
		public class MediumMG : CombatCommander.Weapon {
			public MediumMG() : base(Russian.Instance, 6, 10, -2, 1, 4, 7, 10, false, true) { }
		}



	}
}
