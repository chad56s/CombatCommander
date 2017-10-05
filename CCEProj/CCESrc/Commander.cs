using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {
	
	public enum POSTURE { ATTACK, DEFEND, RECON }

    public partial class Gameboard
    {
        /*
         * This is a class used by Gameboard to represent the components belonging to a particular player. This
         * class has no knowledge of the rules or any responsibility in enforcing them whatsoever. It's just a
         * place to conveniently and collectively track the components that both players will have.
         * 
         * This includes:
         *  Pieces on the board
         *  Pieces on the casualty track
         *  Pieces waiting to be set up or reinforcements
         *  
         *  Radios
         *  
         *  Draw pile
         *  Hand
         *  Discard pile
         * 
         * It is NOT a class that is used by the actual players of the game (see Player)
         */
        private class Commander
        {

            private FACTION faction;

            private List<ObjectiveChit> mySecretObjectives;

            private Scenario.SideSetup setupInfo;

            private Stack mapPieces;
            private Stack setupPieces;
            private Stack eliminatedPieces;
            private Stack reinforcementPieces;

            private Deck drawPile;
            private Deck cardsInHand;
            private Deck discardPile;

            private int _hand_size;

            private bool _hasInitiative;

            public Commander(FACTION f)
            {
                mySecretObjectives = new List<ObjectiveChit>();
                mapPieces = new Stack();
                setupPieces = new Stack();
                eliminatedPieces = new Stack();
                reinforcementPieces = new Stack();

                cardsInHand = new Deck();
                discardPile = new Deck();

                faction = f;

                _hasInitiative = false;

                _hand_size = 0;
            }

            public FACTION Faction
            {
                get { return faction; }
            }

            public int HandSize
            {
                get { return _hand_size > 0 ? _hand_size : Rules.DefaultHandSize(Posture); }
                set { _hand_size = value; }
            }

            public Nationality Nation
            {
                get { return setupInfo.Nation; }
            }

            public int NumOrders
            {
                get { return setupInfo.NumOrders; }
            }

            public POSTURE Posture
            {
                get { return setupInfo.Posture; }
            }

            public TROOP_QUALITY TroopQuality
            {
                get { return setupInfo.Quality; }
            }

            public Scenario.SideSetup SetupInfo
            {
                get { return setupInfo; }
            }

            public bool HasInitiative {
                get { return _hasInitiative; }
                set { _hasInitiative = value; }
            }

            /*
             * 
             * MOVE INTO Player and or GameManager
             * 
             * public void Prepare(Scenario.SideSetup si)
            {

                setupInfo = si;

                if (setupInfo.Faction == Faction)
                {

                    drawPile = Nation.CreateNewDeck();

                    //generate the forces
                    Type nationType = Nation.GetType();

                    foreach (var piece in setupInfo.Pieces)
                    {

                        Type v = Type.GetType(nationType.FullName + "+" + piece.Name);

                        //TODO: Each nationality should have a counter mix from which to pull these pieces
                        //in order to enforce limits
                        try
                        {
                            for (var n = 0; n < piece.Number; n++)
                            {
                                setupPieces.Add((Piece)Activator.CreateInstance(v));
                            }
                        }
                        catch (Exception e)
                        {
                            throw new ArgumentException(String.Format("Can't create piece of type {0}+{1}", nationType.FullName, piece.Name));
                        }
                    }

                    foreach (var o in setupInfo.Objectives)
                    {
                        DrawObjective(o);
                    }
                }
                else
                    throw new ArgumentException(String.Format("Setup information for {0} contains setup information for {1}", Faction.ToString(), setupInfo.Faction.ToString()));

            }
            **/

            public void SetUp()
            {

            }

            public void SetUpFortifications()
            {

            }

            public void DrawHand()
            {
                while (cardsInHand.Count < HandSize)
                {
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
            public List<ObjectiveChit_PW> WrapObjectives()
            {
                List<ObjectiveChit_PW> wrapped = new List<ObjectiveChit_PW>();

                foreach (var o in mySecretObjectives)
                {
                    wrapped.Add(new ObjectiveChit_PW(faction, o));
                }

                return wrapped;
            }
        }
    }
}
