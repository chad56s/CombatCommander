using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using CW_Utils;

namespace CombatCommander {

	public class Game {

        /*
         * Broker
         */
        private GameManager _gm;

		/*
		 * Players
		 * (representation of a player and what its pieces are, etc)
		 */
		private Commander _commander1, _commander2, _commander_axis, _commander_allies;
		private Commander _hasInitiative;

		/*
		 * Components setup
		 * 
		 */
		private List<ObjectiveChit> _objectivesDrawCup;
        private TimeTrack _time_track;

		/*
		 * Scenario information
		 * 
		 */
		private Scenario _scenario;
		private Map _gameMap;

		/*
		 * Other game information
		 * 
		 */
		private List<ObjectiveChit> _openObjectives;
		private int _vp_marker;
		private bool _game_over;

		public Game(string scenario, GameManager gm) {

            _gm = gm;

			_commander_axis = new Commander(FACTION.AXIS, this);
			_commander_allies = new Commander(FACTION.ALLIES, this);

			_objectivesDrawCup = new List<ObjectiveChit>();
			_openObjectives = new List<ObjectiveChit>();

			_scenario = Scenario.LoadByString(scenario);
			_gameMap = _scenario.ScenarioMap;
			_time_track = new TimeTrack(_scenario.TimeStart, _scenario.SuddenDeath);
			_game_over = false;
			
			if (_scenario.FirstPlayer == FACTION.AXIS) {
				_commander1 = _commander_axis;
				_commander2 = _commander_allies;
			}
			else {
				_commander1 = _commander_allies;
				_commander2 = _commander_axis;
			}

			if (_scenario.Initiative == _commander1.Faction)
				_hasInitiative = _commander1;
			else
				_hasInitiative = _commander2;

		}

        /*
         * 
         * Functions that are appropriate for the GameManager to call to retrieve information about the state of the game
         * 
         */

		public bool GameOver {
			get { return _game_over; }
		}
		public int Year {
			get { return _scenario.Year; }
		}
		public int VPMarker {
			get { return _vp_marker; }
            private set { _vp_marker = value; }
		}

        public Nationality GetNationalityByFaction(FACTION f) {
            return GetCommanderByFaction(f).Nation;
        }
        public List<ObjectiveChit_PW> GetObjectivesByFaction(FACTION f) {
            return GetCommanderByFaction(f).WrapObjectives();
        }

        /************************************************
         * 
         *  END OF gameManager-ish functions
         * 
         ************************************************/


		public void AddOpenObjective(ObjectiveChit obj) {
			_openObjectives.Add(obj);
			//TODO: update the game state so that the other player can be made aware
		}

		public ObjectiveChit FindObjectiveChit(char letter) {
			ObjectiveChit chit = null;

			if (letter == '?') {
				if (_objectivesDrawCup.Count > 0) {
					int n = Randomizer.get(0, _objectivesDrawCup.Count - 1);
					chit = _objectivesDrawCup[n];
					_objectivesDrawCup.RemoveAt(n);
				}
				else {
					throw new EmptyObjectivesCup();
				}
			}
			else {
				chit = _objectivesDrawCup.Find(c => c.Letter == letter);
			}
			return chit;
		}

		public ObjectiveChit DrawObjective(char letter = '?') {

			ObjectiveChit chit = null;
			try {
				chit = FindObjectiveChit(letter);
			}
			catch (EmptyObjectivesCup e) {
				return null;
			}

			//link the map objective to this objective chit
			for (var i = 1; i <= 5; i++) {
				if (chit.AppliesToObjectiveNumber(i)) {
					_gameMap.GetObjective(i).AddGameObjective(chit);
				}
			}

			return chit;
		}

		public ObjectiveChit DrawOpenObjective(char letter = '?') {
			ObjectiveChit chit = DrawObjective(letter);
			chit.IsSecret = false;
			this._openObjectives.Add(chit);
			return chit;
		}

		public Commander GetCommanderByFaction(FACTION f) {
			return f == FACTION.ALLIES ? _commander_allies : (f == FACTION.AXIS ? _commander_axis : null);
		}

		public void GiveInitiative(Commander p) {
			_hasInitiative = p;
		}

		public bool HasInitiative(Commander p) {
			return p == _hasInitiative;
		}

		//player p is passing the initiative to other player
		public void PassInitiative(Commander p) {
			if (HasInitiative(p)) {
				if (HasInitiative(_commander_axis))
					_hasInitiative = _commander_allies;
				else
					_hasInitiative = _commander_axis;
			}
		}

		public Commander SetsUpFirst() {
			return GetCommanderByFaction(_scenario.SetupFirst);
		}


		public void Start() {

			PlayersSetUp();  //deploy initial forces, draw opening hands, put down fortifications, etc

			do {

				_commander1.TakeTurn();
				if (!GameOver)
					_commander2.TakeTurn();

			} while (!GameOver);

		}

		public int VPGive(Commander p, int pts) {
			if (p == _commander1)
				VPMarker -= pts;
			else if (p == _commander2)
                VPMarker += pts;
			else
				throw new ArgumentException("Who is this player that is getting points?");
			return VPMarker;
		}
		
		public int VPTake(Commander p, int pts) {
			if (p == _commander1)
                VPMarker += pts;
			else if (p == _commander2)
                VPMarker -= pts;
			else
				throw new ArgumentException("Who is this player that is losing points?");
			return VPMarker;
		}

        public int VPSwitch(Commander from, Commander to, int pts) {
            VPTake(from, pts);
            VPGive(to, pts);
            return VPMarker;
        }

		/*
		 * PRIVATE METHODS
		 * 
		 */
		private void PopulateObjectivesCup() {
			_objectivesDrawCup = new List<ObjectiveChit>();
			_objectivesDrawCup.Add(new MapObjectiveChit('A', ObjectiveChit.OBJECTIVE_1, 1, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('B', ObjectiveChit.OBJECTIVE_2, 1, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('C', ObjectiveChit.OBJECTIVE_3, 1, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('D', ObjectiveChit.OBJECTIVE_4, 1, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('E', ObjectiveChit.OBJECTIVE_5, 1, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('F', ObjectiveChit.OBJECTIVE_2, 2, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('G', ObjectiveChit.OBJECTIVE_3, 2, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('H', ObjectiveChit.OBJECTIVE_4, 2, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('J', ObjectiveChit.OBJECTIVE_5, 2, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('K', ObjectiveChit.OBJECTIVE_3, 3, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('L', ObjectiveChit.OBJECTIVE_4, 3, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('M', ObjectiveChit.OBJECTIVE_5, 3, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('N', ObjectiveChit.OBJECTIVE_4, 4, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('P', ObjectiveChit.OBJECTIVE_5, 4, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('Q', ObjectiveChit.OBJECTIVE_5, 5, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('R', ObjectiveChit.OBJECTIVE_5, 10, false, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('S', ObjectiveChit.OBJECTIVE_1 | ObjectiveChit.OBJECTIVE_2 | ObjectiveChit.OBJECTIVE_3 | ObjectiveChit.OBJECTIVE_4 | ObjectiveChit.OBJECTIVE_5, 1, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('T', ObjectiveChit.OBJECTIVE_1 | ObjectiveChit.OBJECTIVE_2 | ObjectiveChit.OBJECTIVE_3 | ObjectiveChit.OBJECTIVE_4 | ObjectiveChit.OBJECTIVE_5, 2, true, false));
			_objectivesDrawCup.Add(new MapObjectiveChit('U', ObjectiveChit.OBJECTIVE_1 | ObjectiveChit.OBJECTIVE_2 | ObjectiveChit.OBJECTIVE_3 | ObjectiveChit.OBJECTIVE_4 | ObjectiveChit.OBJECTIVE_5, 3, true, false));
			_objectivesDrawCup.Add(new SuddenDeathObjectiveChit('V', ObjectiveChit.OBJECTIVE_1 | ObjectiveChit.OBJECTIVE_2 | ObjectiveChit.OBJECTIVE_3 | ObjectiveChit.OBJECTIVE_4 | ObjectiveChit.OBJECTIVE_5, 0, false, true));
			_objectivesDrawCup.Add(new ExitObjectiveChit('W', 2, false));
			_objectivesDrawCup.Add(new EliminationObjectiveChit('X', 2, false));

			
		}

        private void GameSetUp() {

            //initialize the VPs
            VPGive(_commander_axis, _scenario.AxisSetup.StartVP);
            VPGive(_commander_allies, _scenario.AlliesSetup.StartVP);

            //Put all the chits in the cup 
            PopulateObjectivesCup();
            //now pull out all of the open objectives
            foreach (var o in _scenario.OpenObjectives) {
                DrawOpenObjective(o);
            }

            foreach (var o in _scenario.ObjControl) {
                _gameMap.GetObjective(o.number).Owner = o.faction;
            }

            _commander_axis.Prepare(_scenario.AxisSetup);
            _commander_allies.Prepare(_scenario.AlliesSetup);

        }

		private void PlayersSetUp() {

			Commander c1 = SetsUpFirst();
            Commander c2 = (c1 == _commander_axis) ? _commander_allies : _commander_axis;
			if (c1 == null) {
                c1 = _commander_axis;
                c2 = _commander_allies;
			}

            c1.SetUp();
            c2.SetUp();
            c1.SetUpFortifications();
            c2.SetUpFortifications();
            c1.DrawHand();
            c2.DrawHand();


		}

		/*
		 * 
		 * Game-based Exceptions
		 * 
		 * 
		 */
		public class EmptyObjectivesCup : System.ApplicationException {
			public EmptyObjectivesCup() : base() { }
			public EmptyObjectivesCup(string message) : base(message) { }
			public EmptyObjectivesCup(string message, System.Exception inner) : base(message, inner) { }
		}
	}

    public static class Rules {

        public static int DefaultHandSize(POSTURE p) {
            int hs = 0;
            switch (p) {
                case POSTURE.ATTACK:
                    hs = 6;
                    break;
                case POSTURE.DEFEND:
                    hs = 4;
                    break;
                case POSTURE.RECON:
                    hs = 5;
                    break;
            }
            return hs;
        }


    }
}
