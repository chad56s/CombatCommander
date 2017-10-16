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

	public class GameManager {

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
            _axis_agent = new PlayerAgent(this, _axis_player, FACTION.AXIS);
            _allies_player = allies;
            _allies_agent = new PlayerAgent(this, _allies_player, FACTION.ALLIES);

            _scenario = Scenario.LoadByString(scenario);

            _gameboard = new Gameboard();

            _axis_player.Agent = _axis_agent;
            _allies_player.Agent = _allies_agent;

            SetupScenario();
		}

        public void PlayGame() {

            PlayerSetUp(_gameboard.SetsUpFirst().Faction);
            PlayerSetUp(_gameboard.SetsUpSecond().Faction);
            while (!_gameboard.GameOver)
            {
                
            }
        }

        private void SetupScenario() {
            
            //load the map
            //TODO: configuration on where to find the map?
            XDocument xmlMap = null;
            Map m;
            xmlMap = XDocument.Load(String.Format(@"CCE\map{0}.xml", _scenario.ScenarioMap));
            m = new Map();
            m.Load(xmlMap);

            //fill the objective cup
            _gameboard.PopulateObjectivesCup();

            //set up the markers
            _gameboard.Year = _scenario.Year;
            _gameboard.VPGive(FACTION.AXIS, _scenario.AxisSetup.StartVP);
            _gameboard.VPGive(FACTION.ALLIES, _scenario.AlliesSetup.StartVP);
            _gameboard.SetTimeMarkers(_scenario.TimeStart, _scenario.SuddenDeath);

            //Prepare commanders' decks, setup pieces, counter mixes, decks, posture, troop quality, etc.
            _gameboard.GetCommander(FACTION.AXIS).Prepare(_scenario.AxisSetup);
            _gameboard.GetCommander(FACTION.ALLIES).Prepare(_scenario.AlliesSetup);

            //give initiative to commander as dictated by scenario
            _gameboard.GiveInitiative(_scenario.Initiative);
            
            //draw the open objectives
            //TODO: remove objectives specified by the scenario
            foreach (var o in _scenario.OpenObjectives) {
                _gameboard.DrawOpenObjective(o);
            }
            //draw the secret objectives
            foreach (var o in _scenario.AxisSetup.Objectives) {
                _gameboard.DrawSecretObjective(FACTION.AXIS, o);
            }
            foreach (var o in _scenario.AlliesSetup.Objectives) {
                _gameboard.DrawSecretObjective(FACTION.ALLIES, o);
            }
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


		/*
		 * 
		 * GAME STATE QUERIES
		 * 
		 * Methods which ask the game for information about its state
		 */
        public Nationality GetNationalityByFaction(FACTION f) {
            return _gameboard.GetNationalityByFaction(f);
        }
        public List<ObjectiveChit_PW> GetObjectivesByFaction(FACTION f) {
            return _gameboard.GetObjectivesByFaction(f);
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
        private GameManager _gm;

        public PlayerAgent(GameManager gm, Player p, FACTION f)
        {
            _gm = gm;
            _player = p;
            _faction = f;
        }

        private GameManager GM {
            get { return _gm; }
        }

        private FACTION F {
            get { return _faction; }
        }

        public Nationality GetMyNationality() {
            return GM.GetNationalityByFaction(F);
        }


        public List<ObjectiveChit_PW> GetMyObjectives() {
            return GM.GetObjectivesByFaction(F);
        }
    }
}
