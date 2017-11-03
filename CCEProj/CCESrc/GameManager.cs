using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CombatCommander {

	/*
	 * 
	 * CLASS: GameManager
	 * 
     * Responsible for setup and maintenance of the Gameboard. Responsible for checking legality of and executing
     * Players' wishes. Responsible for checking game state and querying Players at appropriate times. Responsible
     * for honoring parameters of the Scenario.
     * 
	 */

	public partial class GameManager {

        private Gameboard _gameboard;

        /*
         * Scenario information
         * 
         */
        private Scenario _scenario;

		private Player _axis_player, _allies_player;
        private PlayerAgent _axis_agent, _allies_agent;

		public GameManager(String scenario, Player axis, Player allies) {
            _axis_player = axis;
            _allies_player = allies;

            _scenario = Scenario.LoadByString(scenario);

            _axis_agent = new PlayerAgent(this, _axis_player, FACTION.AXIS, _scenario.AxisSetup.Nation);
            _allies_agent = new PlayerAgent(this, _allies_player, FACTION.ALLIES, _scenario.AlliesSetup.Nation);

            _gameboard = new Gameboard();

            _axis_player.Agent = _axis_agent;
            _allies_player.Agent = _allies_agent;

            SetupScenario();
            PlayersSetUp();
            PlayGame();
		}
        
        private void SetupScenario() {
            
            //load the map
            //TODO: configuration on where to find the map?
            XDocument xmlMap = null;
            Map m;
            List<ObjectiveChit> drawnOpenObj = new List<ObjectiveChit>();
            ObjectiveChit drawnChit;
            xmlMap = XDocument.Load(String.Format(@"CCE\map{0}.xml", _scenario.ScenarioMap));
            m = new Map();
            m.Load(xmlMap);

            _gameboard.map = m;

            //fill the objective cup
            _gameboard.PopulateObjectivesCup();

            //set up the markers
            _gameboard.Year = _scenario.Year;
            _gameboard.VPGive(FACTION.AXIS, _scenario.AxisSetup.StartVP);
            _gameboard.VPGive(FACTION.ALLIES, _scenario.AlliesSetup.StartVP);
            _gameboard.SetTimeMarkers(_scenario.TimeStart, _scenario.SuddenDeath);

            //Prepare commanders' decks, setup pieces, counter mixes, decks, posture, troop quality, etc.
            _gameboard.GetCommander(FACTION.AXIS).Prepare(_scenario.AxisSetup, GameManager.Rules.HandSize(_scenario.AxisSetup.Posture));
            _gameboard.GetCommander(FACTION.ALLIES).Prepare(_scenario.AlliesSetup, GameManager.Rules.HandSize(_scenario.AlliesSetup.Posture));

            //give initiative to commander as dictated by scenario
            _gameboard.GiveInitiative(_scenario.Initiative);
            
            //draw the open objectives
            //TODO: remove objectives specified by the scenario
            foreach (var o in _scenario.OpenObjectives) {
                drawnChit = _gameboard.DrawOpenObjective(o);
            }
            //draw the secret objectives - track the open ones so we can adjust VPs afterward
            foreach (var o in _scenario.AxisSetup.Objectives) {
                drawnChit = _gameboard.DrawSecretObjective(FACTION.AXIS, o);
                if (!drawnChit.IsSecret)
                    drawnOpenObj.Add(drawnChit);
            }
            foreach (var o in _scenario.AlliesSetup.Objectives) {
                drawnChit = _gameboard.DrawSecretObjective(FACTION.ALLIES, o);
                if (!drawnChit.IsSecret)
                    drawnOpenObj.Add(drawnChit);
            }

            //tell the gameboard who the owner is - that's where we'll keep track. And it kind of represents that little owner chit
            //that goes on the board.
            //at the same time, award vps for any drawn open objectives
            foreach (var o in _scenario.ObjControl) {
                _gameboard.SetObjectiveOwner(o.number, o.faction);
                foreach (var d in drawnOpenObj) {
                    if (d.AppliesToObjectiveNumber(o.number))
                        _gameboard.VPGive(o.faction, d.Value);
                }
            }

        }

        public void PlayersSetUp() {
            /*
             * Check to see if someone sets up first. 
             * If they do, collect their set up info and apply it to the board.
             * Then have the other player do the same
             * If nobody sets up first, collect from one player, but don't apply to the board.
             * Then collect from the other and then apply all to the board.
             */
        }

        public void PlayGame() {

        }



        private Player AxisPlayer {
            get { return _axis_player; }
        }

        private Player AlliesPlayer {
            get { return _allies_player; }
        }

        public PlayerAgent AxisAgent {
            get { return _axis_agent; }
        }

        public PlayerAgent AlliesAgent {
            get { return _allies_agent; }
        }

        private Player GetPlayerByFaction(FACTION f) {
            Player p = null;

            switch (f)
            {
                case FACTION.AXIS:
                    p = AxisPlayer;
                    break;
                case FACTION.ALLIES:
                    p = AlliesPlayer;
                    break;
                default:
                    throw new ArgumentException("Bad Player");
            }

            return p;
        }
        
		/*
		 * 
		 * COMMANDER INSTRUCTIONS
		 * 
		 * Methods which instruct a commander to do a particular task
		 */
		public void PlayerSetUp(FACTION f) {
			GetPlayerByFaction(f).SetUp();
		}

        public List<ObjectiveChit_PW> GetObjectivesByFaction(FACTION f) {
            return _gameboard.GetSecretObjectivesByFaction(f);
        }

	}

    /*
     * Class: PlayerAgent
     * 
     * brokers requests from the players to the GameManager
     * 
     */
    public class PlayerAgent {

        private Player _player;
        private FACTION _faction;
        private static Nationality _nationality;
        private GameManager _gm;

        public PlayerAgent(GameManager gm, Player p, FACTION f, Nationality n)
        {
            _gm = gm;
            _player = p;
            _faction = f;
            _nationality = n;
        }

        private GameManager GM {
            get { return _gm; }
        }

        private FACTION F {
            get { return _faction; }
        }

        public Nationality GetMyNationality() {
            return _nationality;
        }


        public List<ObjectiveChit_PW> GetMyObjectives() {
            return GM.GetObjectivesByFaction(F);
        }
    }
}
