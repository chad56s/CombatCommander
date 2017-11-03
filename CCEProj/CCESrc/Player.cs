using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {

    /*
     * CLASS: Player
     * 
     * This class represents the entity making the decisions for a particular FACTION and Nationality in a given game. This
     * might be a human or an AI. It communicates intentions through the PlayerAgent and is delivered instructions from that
     * same PlayerAgent as well.
     * 
     */

	public abstract class Player {
		
        private PlayerAgent _agent;
		private FACTION _faction;
        private Nationality _nationality;

        private List<ObjectiveChit_PW> myObjectives; //holds all objectives known about (secret and open)
        
		public Player(FACTION f) {
			_faction = f;
		}

		public FACTION Faction {
			get { return _faction; }
		}

        public Nationality Nationality {
            get { return _nationality; }
            private set { _nationality = value; }
        }

		public PlayerAgent Agent {
			get { return _agent; }
			set { _agent = value; }
		}

        public virtual void Prepare() {
            Nationality = Agent.GetMyNationality();
            myObjectives = Agent.GetMyObjectives();
        }
        
		public abstract void SetUp();
		public abstract void SetUpFortifications();
		public abstract void TakeTurn();
	}


	public class ComputerPlayer : Player {

		public ComputerPlayer(FACTION f)
			: base(f) {
		}

        public override void Prepare() {
            base.Prepare();
        }

		public override void SetUp() {
		}

		public override void SetUpFortifications() {

		}

		public override void TakeTurn() {

		}
	}

	public class HumanPlayer : Player {

		public HumanPlayer(FACTION f)
			: base(f) {
		}

        public void Prepare() {
            base.Prepare();
        }

		public override void SetUp() {

		}

		public override void SetUpFortifications() {

		}

		public override void TakeTurn() {

		}
	}
}
