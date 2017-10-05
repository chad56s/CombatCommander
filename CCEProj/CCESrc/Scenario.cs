using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Xml;
using System.Xml.Linq;

using System.IO;

namespace CombatCommander {

	public enum MAP_ORIENTATION_AXIS { NORTH, EAST, SOUTH, WEST }

    /*
     * Scenario: represents the set-up and special instructions for a game.
     * 
     *  PieceSpecs: represents the type and amount of a given piece in set-up (e.g. 3 Militia units, 1 Heavy Machine Gun weapon, 1 Sgt. Smith unit, etc)
     *  ObjectiveControl: which faction owns an objective to start the game
     *  SideSetup: all of the set up information for a side: nationality, pieces, map edge, troop quality, objective chits, etc.
     */
	public class Scenario {

		public class PieceSpecs {

			private static Regex reWhitespace = new Regex(@"\W");

			private string _type;
			private string _name;
			private int _number;

			public PieceSpecs(string t, string name, int num) {
				_type = t;
				_name = name;
				_number = num;
			}

			public PieceSpecs(string t, string name) : this(t, name, 1) { }

			public string Type {
				get {
					StringBuilder type = new StringBuilder(_type.ToLower());
					type[0] = char.ToUpper(type[0]);
					return type.ToString();
				}
			}

			public string Name {
				get {
					return reWhitespace.Replace(_name, "");
				}
			}

			public int Number {
				get {
					return _number;
				}
			}
		}

		public class ObjectiveControl {

			public ObjectiveControl(string f, string n) {
				try {
					faction = CC_Utils.StringToFaction(f);
					number = int.Parse(n);
				}
				catch (Exception e) {
					throw new ArgumentException(String.Format("Could not create objectiveControl object for: {0} to {1}", n, f));
				}
			}

			public FACTION faction;
			public int number;
		}

		public class SideSetup {

			Nationality _nationality;
			FACTION _faction;

			MAP_ORIENTATION_AXIS _map_edge;
			TROOP_QUALITY _quality;
			POSTURE _posture;
			List<char> _objectives;
			int _num_orders;
			int _surrender_level;
			int _vps;
			List<PieceSpecs> _pieces;

			public SideSetup(Nationality n, MAP_ORIENTATION_AXIS map_edge, TROOP_QUALITY q, int num_orders, POSTURE p, List<char> obj, int surrender_level, int vps, List<PieceSpecs> pieces) {
				_nationality = n;
				_faction = _nationality.Faction;
				_map_edge = map_edge;
				_quality = q;
				_num_orders = num_orders;
				_posture = p;
				_objectives = obj;
				_surrender_level = surrender_level;
				_vps = vps;
				_pieces = pieces;
			}
			public static SideSetup LoadFromXML(XElement xmlSetup) {

                //TODO: detect and handle invalid set ups

				Nationality n;
				string n_string;
				FACTION f;
				MAP_ORIENTATION_AXIS m_o;
				TROOP_QUALITY q;
				POSTURE p;
				List<char> o = new List<char>();
				int n_o, s_l;
				List<PieceSpecs> pieces = new List<PieceSpecs>();
				int vps = n_o = s_l = 0;
				XAttribute xAttr;
				int numPieces;

				n_string = xmlSetup.Element("nationality").Value.ToUpper();
                switch (n_string) {   
                    case "AMERICAN": n = American.Instance; break;
                    case "GERMAN": n = German.Instance; break;
                    case "RUSSIAN": n = Russian.Instance; break;
					default: throw new ArgumentException(String.Format("Unknown Nationality in Scenario setup: {0}", n_string));
				}

				f = CC_Utils.StringToFaction(xmlSetup.Attribute("side").Value, true);

				if (f != n.Faction)
					throw new ArgumentException(String.Format("Invalid faction for nationality {0} in scenario setup", n_string));

				m_o = (MAP_ORIENTATION_AXIS)Enum.Parse(typeof(MAP_ORIENTATION_AXIS), xmlSetup.Element("map_edge").Value.ToUpper());
				q = (TROOP_QUALITY)Enum.Parse(typeof(TROOP_QUALITY), xmlSetup.Element("quality").Value.ToUpper());
				p = (POSTURE)Enum.Parse(typeof(POSTURE), xmlSetup.Element("posture").Value.ToUpper());
				IEnumerable<XElement> objectives =
					from el in xmlSetup.Descendants("objective")
					select el;
				foreach (var objective in objectives) {
					char obj = char.Parse(objective.Value);
					o.Add(obj);
				}
				n_o = int.Parse(xmlSetup.Element("orders").Value);
				s_l = int.Parse(xmlSetup.Element("surrender_level").Value);

				IEnumerable<XElement> units =
					from el in xmlSetup.Elements("units").Elements()
					select el;
				foreach (var unit in units) {
					xAttr = unit.Attribute("num");
					numPieces = xAttr == null ? 1 : int.Parse(xAttr.Value);
					pieces.Add(new PieceSpecs(unit.Name.ToString(), unit.Value, numPieces));
				}

				/*
				 * TODO:
				 * 
				 * INITIAL CONTROL OF OBJECTIVES
				 * 
				 */

				return new SideSetup(n, m_o, q, n_o, p, o, s_l, vps, pieces);

			}

			public Nationality Nation {
				get { return _nationality; }
			}
			public FACTION Faction {
				get { return _faction; }
			}
			public MAP_ORIENTATION_AXIS MapEdge {
				get { return _map_edge; }
			}

			public TROOP_QUALITY Quality {
				get { return _quality; }
			}
			public POSTURE Posture {
				get { return _posture; }
			}
			public List<char> Objectives {
				get { return _objectives; }
			}
			public int NumOrders {
				get { return _num_orders; }
			}

			public int SurrenderLevel {
				get { return _surrender_level; }
			}
			public int StartVP {
				get { return _vps; }
			}
			public List<PieceSpecs> Pieces {
				get { return _pieces; }
			}
		}

		int _number;
		string _name;
		Map _map;
		int _year;
		int _time_start;
		int _sudden_death;
		List<char> _open_objectives;
		FACTION _initiative_card, _first_player, _setup_first;
		SideSetup _axis_setup, _allies_setup;
		List<ObjectiveControl> _objective_control;
		
		
		public Scenario(int number,
						string name,
						Map m, 
						int year,
						int time_start,
						int sudden_death,
						List<char> open_objectives,
						List<ObjectiveControl> objective_control,
						FACTION initiative,
						FACTION setup_first,
						FACTION first_player,
						SideSetup axis_setup,
						SideSetup allies_setup) {

			_number = number;
			_name = name;
			ScenarioMap = m;
			Year = year;
			TimeStart = time_start;
			SuddenDeath = sudden_death;
			OpenObjectives = open_objectives;
			ObjControl = objective_control;
			Initiative = initiative;
			SetupFirst = setup_first;
			FirstPlayer = first_player;
			AxisSetup = axis_setup;
			AlliesSetup = allies_setup;
		}

        public static Scenario LoadByString(string scenarioName){
            XDocument xdocScenario = XDocument.Load(String.Format(@"CCE\scenario_{0}.xml", scenarioName));
            return LoadFromXML(xdocScenario);
        }

		public static Scenario LoadFromXML(XDocument xdocScenario) {
			//TODO: Use XMLSerializer to read/write scenarios
			int number, year, time_start, sudden_death, mapNum;
            XElement xmlScenario = xdocScenario.Root;
			string name = "";
			Map map = null;
			List<char> open_objectives = new List<char>();
			List<ObjectiveControl> obj_ctl = new List<ObjectiveControl>();
			SideSetup axis_setup, allies_setup;
			FACTION initiative, setup_first, first_player;

            string curDirectory = Directory.GetCurrentDirectory();
            XDocument xmlMap = null;

			XElement xmlTime = xmlScenario.Element("time");
			XElement xmlInitiative = xmlScenario.Element("initiative_card");
			XElement xmlSetupFirst = xmlScenario.Element("setup_first");
			XElement xmlFirstPlayer = xmlScenario.Element("first_player");
			XElement xmlObjectives = xmlScenario.Element("objectives");
			XElement xmlObjCtrl = xmlScenario.Element("control");
			
			int.TryParse(xmlScenario.Attribute("id").Value, out number);
			name = xmlScenario.Attribute("name").Value;
            
            int.TryParse(xmlScenario.Attribute("map").Value, out mapNum);
            xmlMap = XDocument.Load(String.Format(@"CCE\map{0}.xml", mapNum));
			map = new Map();
			map.Load(xmlMap);

			int.TryParse(xmlScenario.Attribute("year").Value, out year);
			int.TryParse(xmlTime.Attribute("start").Value, out time_start);
			int.TryParse(xmlTime.Attribute("sudden_death").Value, out sudden_death);

			IEnumerable<string> objectives = 
				from el in xmlObjectives.Elements("objective")
				select el.Value;
			foreach( var s in objectives ){
				char c;
				char.TryParse(s, out c);
				open_objectives.Add(c);
			}

			axis_setup = allies_setup = null;
			IEnumerable<XElement> sideSetups =
				from el in xmlScenario.Elements("side_setup")
				select el;
			foreach(var ss in sideSetups) {
				if(ss.Attribute("side").Value.ToUpper() == "AXIS")
					axis_setup = SideSetup.LoadFromXML(ss);
				else
					allies_setup = SideSetup.LoadFromXML(ss);
			}


			List<ObjectiveControl> objCtrl = new List<ObjectiveControl>();
			if(xmlObjCtrl != null)
				objCtrl =
				(from el in xmlObjCtrl.Elements("objective")
				select new ObjectiveControl(el.Value.ToString(), el.Attribute("num").Value.ToString())).ToList();

			initiative = CC_Utils.StringToFaction(xmlFirstPlayer.Value, false);
			setup_first = CC_Utils.StringToFaction(xmlSetupFirst.Value, false);
			first_player = CC_Utils.StringToFaction(xmlInitiative.Value, false);


			return new Scenario(number, name, map, year, time_start, sudden_death, open_objectives, objCtrl, initiative, setup_first, first_player, axis_setup, allies_setup);
		}

		public Map ScenarioMap {
			get { return _map; }
			set { _map = value; }
		}

		public int Year {
			set {
				if (value > 1940 && value < 1946)
					_year = value;
				else
					throw new ArgumentOutOfRangeException(String.Format("Invalid Year for Scenario ({0})", value));
			}
			get { return _year; }
		}

		public int TimeStart {
			get { return _time_start; }
			set {
				if (value >= 0 && value <= 12)
					_time_start = value;
				else
					throw new ArgumentOutOfRangeException(String.Format("Invalid value for Time Start ({0})", value));
			}
		}

		public int SuddenDeath {
			get { return _sudden_death; }
			set {
				if (value >= 0 && value <= 12)
					_sudden_death = value;
				else
					throw new ArgumentOutOfRangeException(String.Format("Invalid value for Sudden Death ({0})", value));
			}
		}

		public List<char> OpenObjectives {
			get { return _open_objectives; }
			set { _open_objectives = value; }
		}

		public List<ObjectiveControl> ObjControl {
			get { return _objective_control; }
			set { _objective_control = value; }
		}

		public FACTION Initiative {
			get { return _initiative_card; }
			set { _initiative_card = value; }
		}

		public FACTION SetupFirst {
			get { return _setup_first; }
			set { _setup_first = value; }
		}

        public FACTION SetupSecond {
            get {
                return SetupFirst == FACTION.AXIS ? FACTION.ALLIES : FACTION.AXIS;
            }
        }

		public FACTION FirstPlayer {
			get { return _first_player; }
			set { _first_player = value; }
		}

		public SideSetup AxisSetup {
			get { return _axis_setup; }
			set {
				if (value.Faction == FACTION.AXIS)
					_axis_setup = value;
				else
					throw new ArgumentException("Axis setup was given a nationality other than AXIS");
			}
		}

		public SideSetup AlliesSetup {
			get { return _allies_setup; }
			set {
				if (value.Faction == FACTION.ALLIES)
					_allies_setup = value;
				else
					throw new ArgumentException("Allies setup was given a nationality other than ALLIES");
			}
		}
	}
}
