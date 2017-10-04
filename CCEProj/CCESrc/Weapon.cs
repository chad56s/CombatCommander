using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {
	public abstract class Weapon : Piece {

		int _break_min, _break_max, _fix_min, _fix_max;

		public Weapon(Nationality nationality, int a, int b, int c, int d, int e, int f, int g, bool u, bool v) {
			_nationality = nationality;

			_firepower = a;
			_range = b;
			_movement = c;
			_fix_min = d;
			_fix_max = e;
			_break_min = f;
			_break_max = g;

			_firepower_broken = 0;
			_range_broken = 0;
			_movement_broken = 0;

			_boxed_firepower = u;
			_boxed_range = v;
			_boxed_movement = false;
			_boxed_firepower_broken = false;
			_boxed_range_broken = false;
			_boxed_movement_broken = false;

			_broken = false;
			_suppressed = false;

			location = null;
		}
	}

	public abstract class Ordnance : Weapon {
		public Ordnance(Nationality nationality, int a, int b, int c, int d, int e, int f, int g, bool u, bool v)
			: base(nationality, a, b, c, d, e, f, g, u, v) { }

		public override int LeaderBonus() {
			return 0;
		}
	}

	public abstract class Radio : Piece {
		public Radio(Nationality nationality, int a) {
			_firepower = a;
		}
		public override int LeaderBonus() {
			return 0;
		}
	}

}
