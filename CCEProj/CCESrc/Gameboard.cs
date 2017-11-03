using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using CW_Utils;


namespace CombatCommander {

    /*
     * CLASS: Gameboard
     * 
     * Represents the physical components including the map, counters, tracks and player decks(?), objective cup, etc.
     * This includes the components' locations on the map, values on the tracks, counter states (broken/etc.)
     * 
     * Should only be able to be controlled by the GameManager (only it can move pieces around, draw objectives, draw cards, etc.)
     * 
     * This class has no knowledge of rules or any kind of responsibility in enforcing them. It is ONLY to represent physical states
     * of components (no matter how illegal they may be).
     * 
     * EXCEPTION: This class will be able to tell whether two hexes have LOS to one another
     */
	public partial class Gameboard {
        
		/*
		 * Commanders
		 * representation of a player and what its pieces are, its draw deck, hand, discard, its counter mix
         * surrender level, posture, troop quality, etc. Basically anything that both players have
		 */
		private Commander CommanderA, CommanderB;

		/*
		 * Components setup
		 * 
		 */
		private List<ObjectiveChit> _objectivesDrawCup;
        private TimeTrack _time_track;

		private Map _gameMap;
        private Dictionary<Map.ObjectiveHexes, FACTION> _objControl;

		/*
		 * Other game information
		 * 
		 */
		private List<ObjectiveChit> _openObjectives;
		private int _vp_marker;
        private int _year;

		public Gameboard() {
            
			_objectivesDrawCup = new List<ObjectiveChit>();
			_openObjectives = new List<ObjectiveChit>();

            CommanderA = new Commander(FACTION.AXIS);
            CommanderB = new Commander(FACTION.ALLIES);

		}

        public void SetTimeMarkers(int time, int suddenDeath) {
            if (_time_track == null)
                _time_track = new TimeTrack(time, suddenDeath);
            else
                _time_track.Time = time;

        }

        /*
         * 
         * Functions for retrieving info on states of the game components
         * 
         */

        public Map map {
            get { return _gameMap; }
            set { _gameMap = value; }
        }

		public int Year {
            get { return _year; }
            set { _year = value; }
		}

		public int VPMarker {
			get { return _vp_marker; }
            private set { _vp_marker = value; }
		}

        public List<ObjectiveChit_PW> GetSecretObjectivesByFaction(FACTION f) {
            return GetCommander(f).WrapObjectives();
        }

        //this function only returns the state of the owner "chit". It does not try to figure out who
        //the owner should be - that's the job of the GameManager
        public FACTION GetObjectiveOwner(int objNum) {
            Map.ObjectiveHexes mapObj = _gameMap.GetObjective(objNum);
            return(_objControl.ContainsKey(mapObj) ? _objControl[mapObj] : FACTION.NONE);
        }

        public void SetObjectiveOwner(int objNum, FACTION owner) {
            _objControl[_gameMap.GetObjective(objNum)] = owner;
        }

        /************************************************
         * 
         *  END OF gameManager-ish functions
         * 
         ************************************************/

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

			return chit;
		}

		public ObjectiveChit DrawOpenObjective(char letter = '?') {
			ObjectiveChit chit = DrawObjective(letter);
			chit.IsSecret = false;
			this._openObjectives.Add(chit);
			return chit;
		}

        public ObjectiveChit DrawSecretObjective(FACTION f, char letter = '?') {
            ObjectiveChit chit = DrawObjective(letter);
            if (!chit.IsSecret)
                this._openObjectives.Add(chit);
            else
                GetCommander(f).secretObjectives.Add(chit);
            return chit;
        }

        public void GiveInitiative(FACTION f) {
            GiveInitiativeToCommander(GetCommander(f));
        }
        public bool HasInitiative(FACTION f) {
            return GetCommander(f).HasInitiative;
        }
        public void GiveInitiativeToCommander(Commander c) {
            c.HasInitiative = true;
            GetOtherCommander(c.Faction).HasInitiative = false;
        }

		//FACTION f is passing the initiative to other FACTION
		public bool PassInitiative(FACTION f) {
            if (HasInitiative(f)) {
                GiveInitiativeToCommander(GetOtherCommander(f));
                return true;
            }
            else
                return false;
		}

		public int VPGive(FACTION f, int pts) {
            Commander p = GetCommander(f);
			if (p == CommanderA)
				VPMarker -= pts;
			else if (p == CommanderB)
                VPMarker += pts;
			else
				throw new ArgumentException("Who is this player that is getting points?");
			return VPMarker;
		}

		
		public int VPTake(FACTION f, int pts) {
			return VPGive(f, -pts);
		}

        public int VPSwitch(FACTION from, FACTION to, int pts) {
            VPTake(from, pts);
            return VPGive(to, pts);
        }

		/*
		 * PRIVATE METHODS
		 * 
		 */

        public Commander GetCommander(FACTION f) {
            Commander c;
            c = (f == CommanderA.Faction) ? CommanderA : (f == CommanderB.Faction ? CommanderB : null);
            if (c == null)
                throw new ArgumentOutOfRangeException(String.Format("No such Commander for Faction: {0}", f));
            return c;
        }

        public Commander GetOtherCommander(FACTION f) {
            Commander c;
            c = (f == CommanderA.Faction) ? CommanderB : (f == CommanderB.Faction ? CommanderA : null);
            if (c == null)
                throw new ArgumentOutOfRangeException(String.Format("No such Commander for Faction: {0}", f));
            return c;
        }

		public void PopulateObjectivesCup() {
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

			//clear out the open objectives and secret objectives
            _openObjectives = new List<ObjectiveChit>();
            GetCommander(FACTION.AXIS).secretObjectives = new List<ObjectiveChit>();
            GetCommander(FACTION.ALLIES).secretObjectives = new List<ObjectiveChit>();
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

}
