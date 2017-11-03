using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;

namespace CombatCommander {


    public class CounterMix {
        protected Dictionary<Type, List<Piece>> mix; //keyed on Piece Type, contains an array with one element for each of that type in the mix
        protected Nationality nation;

        public CounterMix(Nationality n) {
            nation = n;
        }

        private static Regex reToCounterType = new Regex(@"\W");
        /****
            * Add functions
            * 
            */

        /*
            * Add a piece into the counter mix. Returns true if succesful, false if unsuccessful (meaning it was already in the counter mix)
            */
        public bool Add(Piece p) {
            bool success = false;
            List<Piece> counters;

            if (!mix.TryGetValue(p.GetType(), out counters))
                mix.Add(p.GetType(), counters);

            if (!counters.Contains(p)) {
                counters.Add(p);
                success = true;
            }
            return success;
        }

        public void AddNew(Type pieceType, int num=1) {
            for (int i = 0; i < num; i++ )
                Add((Piece)Activator.CreateInstance(pieceType));
        }

        public void AddNew(String pieceType, int num=1) {
            try {
                AddNew(Type.GetType(nation.GetType() + reToCounterType.Replace(pieceType,"")), num);
            }
            catch (Exception e) {
                throw new ArgumentException(String.Format("Can't create piece of type {0}", pieceType));
            }
        }

        public List<Piece> Pull(Type pieceType, int num=1) {
            List<Piece> mixPieces = null;
            List<Piece> pulledPieces = null;

            if (mix.TryGetValue(pieceType, out mixPieces) && mixPieces.Count >= num) {
                pulledPieces = mixPieces.GetRange(0, num);
                //TODO: get the correct number
                mixPieces.RemoveRange(0, num);
            }
            else if (mixPieces != null) {
                throw new CounterMixException(String.Format("There are only {0} counters in the mix of type {1} (Requested {2})", mixPieces.Count, pieceType.ToString(), num));
            }
            return pulledPieces;
        }

        public List<Piece> Pull(String pieceType, int num = 1) {
            try {
                return Pull(Type.GetType(nation.GetType() + reToCounterType.Replace(pieceType, "")), num);
            }
            catch (Exception e) {
                throw new ArgumentException(String.Format("Can't pull piece of type {0}", pieceType));
            }
        }

        public int Count(Type pieceType) {
            List<Piece> pieces;
            int count = 0;
            if (mix.TryGetValue(pieceType, out pieces))
                count = pieces.Count;
            return count;
        }

        public class CounterMixException : System.ApplicationException {
            public CounterMixException() : base() { }
            public CounterMixException(string message) : base(message) { }
            public CounterMixException(string message, System.Exception inner) : base(message, inner) { }
        }
    }
        
    public sealed class AmericanCounterMix : CounterMix {
        public AmericanCounterMix(Nationality n)
            : base(n) {

            AddNew("Hero", 1);
            AddNew("Private", 1);
            AddNew("CorporalX", 2);
            AddNew("CorporalY", 2);
            AddNew("SergeantX", 4);
            AddNew("SergeantY", 4);
            AddNew("LieutenantX", 2);
            AddNew("LieutenantY", 2);
            AddNew("Captain", 1);

            AddNew("Engineer", 6);
            AddNew("Paratroop", 12);
            AddNew("EliteSquad", 12);
            AddNew("LineSquad", 12);
            AddNew("GreenSquad", 12);

            AddNew("Weapon", 6);
            AddNew("Elite", 8);
            AddNew("Line", 8);
            AddNew("Green", 8);
        }
    }

    public sealed class GermanCounterMix : CounterMix {
        public GermanCounterMix(Nationality n) 
            : base(n) {

        }
    }
    public sealed class RussianCounterMix : CounterMix {
        public RussianCounterMix(Nationality n)
            : base(n) {

        }
    }
}
