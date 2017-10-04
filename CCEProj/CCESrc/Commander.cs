using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {
	
	public enum POSTURE { ATTACK, DEFEND, RECON }

    /*
     * This is a class used by game to represent its players. 
     * 
     * It is NOT a class that is used by the actual players of the game (see Commander)
     */
	public class Commander {

		private Game game;
		private FACTION faction;

		private List<ObjectiveChit> myObjectives;

		private Scenario.SideSetup setupInfo;

		private Stack mapPieces;
		private Stack setupPieces;
		private Stack eliminatedPieces;
		private Stack reinforcementPieces;

		private Deck drawPile;
		private Deck cardsInHand;
		private Deck discardPile;

        private int _hand_size;

		public Commander(FACTION f, Game g) {
			myObjectives = new List<ObjectiveChit>();
			mapPieces = new Stack();
			setupPieces = new Stack();
			eliminatedPieces = new Stack();
			reinforcementPieces = new Stack();

			cardsInHand = new Deck();
			discardPile = new Deck();

			faction = f;
			Game = g;

            _hand_size = 0;
		}

		public FACTION Faction {
			get { return faction; }
		}

		public Game Game {
			set { game = value; }
		}

        public int HandSize {
            get { return _hand_size > 0 ? _hand_size : Rules.DefaultHandSize(Posture); }
            set { _hand_size = value; }
        }

		public Nationality Nation {
			get { return setupInfo.Nation; }
		}

		public int NumOrders {
			get { return setupInfo.NumOrders; }
		}

		public POSTURE Posture {
			get { return setupInfo.Posture; }
		}

		public TROOP_QUALITY TroopQuality {
			get { return setupInfo.Quality; }
		}

		public Scenario.SideSetup SetupInfo {
			get { return setupInfo; }
		}

		public ObjectiveChit DrawObjective(char c='?') {
			ObjectiveChit obj = game.DrawObjective(c);
			obj.DrawnBy = this.Faction;

			if (obj.IsSecret)
				myObjectives.Add(obj);
			else
				game.AddOpenObjective(obj);

			return obj;
		}

		public bool HaveInitiative() {
			return game.HasInitiative(this);
		}

		public void Prepare(Scenario.SideSetup si) {

			setupInfo = si;

			if (setupInfo.Faction == Faction) {

				drawPile = Nation.CreateNewDeck();

				//generate the forces
				Type nationType = Nation.GetType();

				foreach (var piece in setupInfo.Pieces) {

					Type v = Type.GetType(nationType.FullName + "+" + piece.Name);

					//TODO: Each nationality should have a counter mix from which to pull these pieces
					//in order to enforce limits
					try {
						for (var n = 0; n < piece.Number; n++) {
							setupPieces.Add((Piece)Activator.CreateInstance(v));
						}
					}
					catch (Exception e) {
						throw new ArgumentException(String.Format("Can't create piece of type {0}+{1}", nationType.FullName, piece.Name));
					}
				}

				foreach (var o in setupInfo.Objectives) {
					myObjectives.Add(DrawObjective(o));
				}
			}
			else
				throw new ArgumentException(String.Format("Setup information for {0} contains setup information for {1}", Faction.ToString(), setupInfo.Faction.ToString()));

		}

        public void SetUp() {

        }

		public void SetUpFortifications() {
			
		}

        public void DrawHand(){
            while (cardsInHand.Count < HandSize) {
                //TODO: account for empty deck
                try
                {
                    cardsInHand.Add(drawPile.Draw());
                }
                catch (Deck.EmptyDeckException e)
                {
                    if (discardPile.Count > 0)
                    {
                        drawPile = discardPile;
                        discardPile = new Deck();
                    }
                    else
                        throw e;
                }
            }

        }

        public void TakeTurn()
        {

		}


        /*********
         * 
         * Functions that wrap our data for consumption by a player
         * 
         ************/
        public List<ObjectiveChit_PW> WrapObjectives() {
            List<ObjectiveChit_PW> wrapped = new List<ObjectiveChit_PW>();
            
            foreach (var o in myObjectives) {
                wrapped.Add(new ObjectiveChit_PW(faction,o));
            }

            return wrapped;
        }
    }

}
