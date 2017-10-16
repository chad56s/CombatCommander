using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CW_Utils;

namespace CombatCommander {

	public enum ORDERS {
		ADVANCE,
		ARTILLERY_DENIED,
		ARTILLERY_REQUEST,
		COMMAND_CONFUSION,
		FIRE,
		MOVE,
		RECOVER,
		ROUT
	}

	public enum ACTIONS {
		AMBUSH,
		ASSAULT_FIRE,
		BORE_SIGHTING,
		COMMAND_CONFUSION,
		CONCEALMENT,
		CROSSFIRE,
		DEMOLITIONS,
		DIG_IN,
		OPPORTUNITY_FIRE,
		HAND_GRENADES,
		HIDDEN_ENTRENCHMENTS,
		HIDDEN_MINES,
		HIDDEN_PILLBOX,
		HIDDEN_UNIT,
		HIDDEN_WIRE,
		LIGHT_WOUNDS,
		MARKSMANSHIP,
		NO_QUARTER,
		SMOKE_GRENADES,
		SPRAY_FIRE,
		SUSTAINED_FIRE
	}

	public enum EVENTS {
		AIR_SUPPORT,
		BATTLE_HARDEN,
		BATTLEFIELD_INTEGRITY,
		BLAZE,
		BOOBY_TRAP,
		BREEZE,
		COMMAND_AND_CONTROL,
		COMMISSAR,
		COWER,
		DEPLOY,
		DUST,
		ELAN,
		ENTRENCH,
		FIELD_PROMOTION,
		FOG_OF_WAR,
		HERO,
		INFILTRATION,
		INTERDICTION,
		INTERROGATION,
		KIA,
		MALFUNCTION,
		MEDIC,
		MISSION_OBJECTIVE,
		PRISONERS_OF_WAR,
		RECONNAISSANCE,
		REINFORCEMENTS,
		RUBBLE,
		SAPPERS,
		SCROUNGE,
		SHELL_SHOCK,
		SHELLHOLES,
		STRATEGIC_OBJECTIVE,
		SUPPRESSING_FIRE,
		WALKING_WOUNDED,
		WHITE_PHOSPHORUS
	}

	public enum TRIGGERS {
		NONE,
		EVENT,
		JAMMED,
		SNIPER,
		TIME
	}

	public class Card {

		private Nationality _nation;
		private int _number;

		private ORDERS _order;
		private ACTIONS _action;
		private EVENTS _event;
		private String _hex;
		private int _white_die;
		private int _red_die;
		private TRIGGERS _trigger;

		public Card(Nationality nation, int number, ORDERS order, ACTIONS action, EVENTS ev, String hex, int white, int red, TRIGGERS trigger) {
			_nation = nation;
			_number = number;
			_order = order;
			_action = action;
			_event = ev;
			_hex = hex;
			_white_die = white;
			_red_die = red;
			_trigger = trigger;
		}

		public override string ToString() {
			return Nation.Abbrev.ToString() + "-" + Number.ToString("00");
		}
		public ACTIONS Action {
			get { return _action; }
		}
		public int DieRoll {
			get { return _white_die + _red_die; }
		}
		public EVENTS Event {
			get { return _event; }
		}
		public String Hex {
			get { return _hex; }
		}
		public Nationality Nation {
			get { return _nation; }
		}
		public int Number {
			get { return _number; }
		}
		public ORDERS Order {
			get { return _order; }
		}
		public int Red {
			get { return _red_die; }
		}
		public TRIGGERS Trigger {
			get { return _trigger; }
		}
		public int White {
			get { return _white_die; }
		}

	}

}
