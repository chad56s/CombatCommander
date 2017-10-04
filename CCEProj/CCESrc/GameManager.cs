using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {

	/*
	 * 
	 * CLASS: GameManager
	 * 
	 * Acts as a layer between the player and the game implementation
	 * Responsible for conveying intentions of the player to the game (delivered via PlayerAgent) which will check legality and then execute
	 * Responsible for conveying queries/clarifications to the player by the game engine when appropriate
	 * 
	 */

	public class GameManager {

		private Game game;

		private Player _axis_player, _allies_player;
        private PlayerAgent _axis_agent, _allies_agent;

		public GameManager(String scenario, Player axis, Player allies) {
            _axis_player = axis;
            _axis_agent = new PlayerAgent(this, _axis_player, FACTION.AXIS);
            _allies_player = allies;
            _allies_agent = new PlayerAgent(this, _allies_player, FACTION.ALLIES);

            game = new Game(scenario, this);

            _axis_player.Agent = _axis_agent;
            _allies_player.Agent = _allies_agent;

		}

        public void PlayGame() {

            PlayerSetUp(game.SetsUpFirst().Faction);
            while (!game.GameOver) {
                
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
            return game.GetNationalityByFaction(f);
        }
        public List<ObjectiveChit_PW> GetObjectivesByFaction(FACTION f) {
            return game.GetObjectivesByFaction(f);
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
