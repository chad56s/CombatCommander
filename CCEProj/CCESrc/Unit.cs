using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;


namespace CombatCommander {

	public abstract class Piece {

		protected Nationality _nationality;
        protected string _name;

		protected int _firepower;
		protected int _range;
		protected int _movement;

		protected bool _boxed_firepower;
		protected bool _boxed_range;
		protected bool _boxed_movement;

		protected int _firepower_broken;
		protected int _range_broken;
		protected int _movement_broken;

		protected bool _broken;
		protected bool _suppressed;

		protected bool _boxed_firepower_broken;
		protected bool _boxed_range_broken;
		protected bool _boxed_movement_broken;

		public Map.Hex location { get; set; }

		public Nationality Nation {
			get { return _nationality; }
		}
        public string Name {
            get { return _name; }
        }
		public virtual int Firepower {
			get { return PrintedFirepower + LeaderBonus(); }
		}
		public virtual int Range {
			get { return PrintedRange + LeaderBonus(); }
		}
		public virtual int Movement {
			get { return PrintedMovement + LeaderBonus(); }
		}
		public virtual int Morale {
			get { throw new UnsupportedUnitProperty("This piece type does not have a Morale"); }
		}
		public virtual int Leadership {
			get { throw new UnsupportedUnitProperty("This piece type does not have a Leadership"); }
		}

		public int PrintedFirepower {
			get { return _broken ? _firepower_broken : _firepower; }
		}
		public int PrintedRange {
			get { return _broken ? _range_broken : _range; }
		}
		public int PrintedMovement {
			get { return _broken ? _movement_broken : _movement; }
		}
		public virtual int PrintedMorale {
			get { throw new UnsupportedUnitProperty("This piece type does not have a Morale"); }
		}
		public virtual int PrintedLeadership {
			get { throw new UnsupportedUnitProperty("This piece type does not have a Leadership"); }
		}

		public virtual bool HasBoxedFirepower() {
			return _broken ? _boxed_firepower_broken : _boxed_firepower;
		}
		public virtual bool HasBoxedRange() {
			return _broken ? _boxed_range_broken : _boxed_range;
		}
		public virtual bool HasBoxedMovement() {
			return _broken ? _boxed_movement_broken : _boxed_movement;
		}

		public virtual int LeaderBonus() {
			return location != null ? location.LeaderBonus() : 0;
		}

		public virtual bool IsLeader() {
			return false;
		}

        public virtual bool IsHero() {
            return false;
        }

		public virtual bool IsSuppressed() {
			return _suppressed;
		}

		public virtual bool IsVeteran() {
			return false;
		}

		/*
		 * STATUS UPDATE METHODS
		 */
		public void Break() {
			_broken = true;
			//TODO: implement elimination
		}
		public void Rally() {
			_broken = false;
		}

		/*
		 * GAME KNOWLEDGE
		 */
		public IEnumerable<Map.Hex> GetFireableHexes() {
			return GetLOSHexes().Intersect(GetRangeHexes());
		}

		public List<Map.Hex> GetLOSHexes() {
			return location.GetAllLOS();
		}

		public List<Map.Hex> GetRangeHexes() {
			return location.GetHexesWithin(Range);
		}

	}

	public enum TROOP_QUALITY { GREEN, LINE, VETERAN }

	public abstract class Unit : Piece {

		private int _morale;
		private int _morale_broken;

		private bool _veteran;

		private Weapon _weapon;

		public Unit(Nationality nationality, string name, int a, int b, int c, int d, int f, int g, int h, int i, bool u, bool v, bool w, bool x, bool y, bool z) {
			_nationality = nationality;
            _name = name;

			_morale = a;
			_firepower = b;
			_range = c;
			_movement = d;
			_morale_broken = f;
			_firepower_broken = g;
			_range_broken = h;
			_movement_broken = i;

			_boxed_firepower = u;
			_boxed_range = v;
			_boxed_movement = w;
			_boxed_firepower_broken = x;
			_boxed_range_broken = y;
			_boxed_movement_broken = z;

			_broken = false;
			_suppressed = false;
			_veteran = false;

			_weapon = null;

			location = null;

		}

		/*
		 * INFORMATION RETRIEVAL METHODS
		 */
		public override int Firepower {
			get { return base.Firepower + StatusAdjustments(); }
		}
		public override int Range {
			get { return base.Range + StatusAdjustments(); }
		}
		public override int Movement {
			get { return base.Movement + StatusAdjustments(); }
		}
		public override int Morale {
			get { return PrintedMorale + StatusAdjustments(); }
		}
		public override int PrintedMorale {
			get { return _broken ? _morale_broken : _morale; }
		}
		public override bool IsVeteran() {
			return _veteran;
		}

		protected virtual int StatusAdjustments() {
			int adj = 0;
			if (IsSuppressed()) adj -= 1;
			if (IsVeteran()) adj += 1;
			return adj;
		}

	}

	public abstract class Leader : Unit {

		protected int _leadership;
		protected int _leadership_broken;

		public Leader(Nationality n, string name, int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, bool u, bool v, bool w, bool x, bool y, bool z) :
			base(n, name, a, b, c, d, f, g, h, i, u, v, w, x, y, z) {
				_leadership = e;
				_leadership_broken = j;
		}

		public override int Leadership {
			get { return PrintedLeadership + StatusAdjustments(); }
		}
		public override int PrintedLeadership {
			get { return _broken ? _leadership_broken : _leadership; }
		}
		public override int LeaderBonus() {
			return 0;
		}
		public override bool IsLeader() {
			return true;
		}

	}

    public abstract class Hero : Leader {
        public Hero(Nationality n, string name, int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, bool u, bool v, bool w, bool x, bool y, bool z) :
            base(n, name, a, b, c, d, e, f, g, h, i, j, u, v, w, x, y, z) { }
        
        public override bool IsHero() {
            return true;
        }
    }

	public abstract class Squad : Unit {
		public Squad(Nationality n, string name, int a, int b, int c, int d, int f, int g, int h, int i, bool u, bool v, bool w, bool x, bool y, bool z) :
			base(n, name, a, b, c, d, f, g, h, i, u, v, w, x, y, z) { }
	}

	public abstract class Team : Unit {
		public Team(Nationality n, string name, int a, int b, int c, int d, int f, int g, int h, int i, bool u, bool v, bool w, bool x, bool y, bool z) :
			base(n, name, a, b, c, d, f, g, h, i, u, v, w, x, y, z) { }
	}

	
	public class Stack : IEnumerable<Piece>{

		private HashSet<Piece> pieces;

		public Stack() { pieces = new HashSet<Piece>();}
		public bool Add(Piece u) { return pieces.Add(u); }
        public void Add(IEnumerable<Piece> s) { pieces.UnionWith(s); }
		public bool Remove(Piece u) { return pieces.Remove(u); }
		public IEnumerator<Piece> GetEnumerator() { return pieces.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
        
	}


	public class UnsupportedUnitProperty : System.ApplicationException {
		public UnsupportedUnitProperty() : base() { }
		public UnsupportedUnitProperty(string message) : base(message) { }
		public UnsupportedUnitProperty(string message, System.Exception inner) : base(message, inner) { }
	}
}
