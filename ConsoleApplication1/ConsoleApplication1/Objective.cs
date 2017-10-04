using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatCommander {

	public abstract class ObjectiveChit {

		public static int OBJECTIVE_1 = 0x01;
		public static int OBJECTIVE_2 = 0X02;
		public static int OBJECTIVE_3 = 0x04;
		public static int OBJECTIVE_4 = 0x08;
		public static int OBJECTIVE_5 = 0x10;

		protected char _letter;
		protected int _numbers;		//which map objectives does this chit apply to?  Bit-wise (54321)

		protected int _value;

		protected bool _isSecret;

		protected bool _allRequired;  //do all objectives need to be captured or just one of them?

		FACTION drawnBy; //who drew the objective

		public ObjectiveChit() {

		}

		public ObjectiveChit(char letter, int numbers, int value, bool secret, bool allRequired) {
			_letter = letter;
			_numbers = numbers;	
			_value = value;			
			_isSecret = secret;
		}

		public FACTION DrawnBy {
			get { return drawnBy; }
			set { drawnBy = value; }
		}
		public bool IsSecret {
			get { return _isSecret; }
			set { _isSecret = value; }
		}

		public char Letter {
			get { return _letter; }
			set { _letter = value; }
		}

		public int Numbers {
			get { return _numbers; }
			set { _numbers = value; }
		}

		public int Value {
			get { return _value; }
			set { _value = value; }
		}

		protected int ToHexRepresentation(int number) {
			int h = 0;
			if (number >= OBJECTIVE_1 && number <= OBJECTIVE_5) {
				h = 1 << (number-1);
			}
			return h;
		}

		public abstract bool AppliesToObjectiveNumber(int n);

		public abstract void Award(FACTION f);

		public abstract int GetKnownValue(FACTION f);

		public abstract int GetPointSwing(FACTION f);

	}

	public class MapObjectiveChit : ObjectiveChit {

		public MapObjectiveChit(char letter, int number, int value, bool secret, bool allRequired) : base(letter, number, value, secret, allRequired) { }

		public override bool AppliesToObjectiveNumber(int n) {
			return (ToHexRepresentation(n) & Numbers) != 0;
		}

		public override void Award(FACTION f) {
			
		}

		public override int GetKnownValue(FACTION f) {
			int v = 0;
			if (!IsSecret || DrawnBy == f)
				v = Value;
			return v;
		}

		public override int GetPointSwing(FACTION f) {
			int v = 0;
			if (!IsSecret || DrawnBy == f)
				v = Value*2;
			return v;
		}
	}

	public class ExitObjectiveChit : ObjectiveChit {

		public ExitObjectiveChit(char letter, int value, bool secret) : base(letter, 0, value, secret, false) { }
		
		public override bool AppliesToObjectiveNumber(int n) {
			return false;
		}
		public override void Award(FACTION f) {

		}
		public override int GetKnownValue(FACTION f) {
			return 4;
		}
        public override int GetPointSwing(FACTION f)
        {
			return 4;
		}
	}

	public class EliminationObjectiveChit : ObjectiveChit {
		public EliminationObjectiveChit(char letter, int value, bool secret) : base(letter, 0, value, secret, false) { }
		
		public override bool AppliesToObjectiveNumber(int n) {
			return false;
		}
        public override void Award(FACTION f)
        {

		}
        public override int GetKnownValue(FACTION f)
        {
			return 4;
		}
        public override int GetPointSwing(FACTION f)
        {
			return 4;
		}
	}

	public class SuddenDeathObjectiveChit : ObjectiveChit {
		public SuddenDeathObjectiveChit(char letter, int number, int value, bool secret, bool allRequired) : base(letter, number, value, secret, allRequired) { }

		public override bool AppliesToObjectiveNumber(int n) {
			return (ToHexRepresentation(n) & Numbers) != 0;
		}
        public override void Award(FACTION f)
        {

		}
		//TODO: figure out something better for this?  Maybe a formula based on end of game closeness, etc?
        public override int GetKnownValue(FACTION f)
        {
			return 0;
		}
        public override int GetPointSwing(FACTION f)
        {
			return 0;
		}
	}
}
