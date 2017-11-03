using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;

using System.Diagnostics;


namespace CombatCommander {

	public enum TERRAIN {
		BLAZE,
		BRUSH,
		BUILDING,
		FIELD,
		GULLY,
		MAJORBRIDGE,
		MARSH,
		OPEN,
		ORCHARD,
		STREAM,
		WATERBARRIER,
		WOODS
	}

	public enum FEATURES {
		NONE,
		CLIFF,
		FENCE,
		HEDGE,
		MINORBRIDGE,
		RAILWAY,
		ROAD,
		SMOKE,
		TRAIL,
		WALL
	}

	public enum COMPASS {
		NORTH,
		NORTHEAST,
		SOUTHEAST,
		SOUTH,
		SOUTHWEST,
		NORTHWEST
	}

	/*
	 * 
	 * 
	 * CLASS MAP
	 * 
	 */
	public class Map {
		public const int Width = 15;
		public const int Height = 10;

		private Hex[,] hexes; //col,row

		private List<ObjectiveHexes> Objectives;
		public ObjectiveHexes GetObjective(int o) { return Objectives[o-1]; }

		public static HexOrderer hexOrder = new HexOrderer();

		public static int FeatureCost(FEATURES f) {
			int c = 0;

			switch (f) {
				case FEATURES.NONE:
				case FEATURES.MINORBRIDGE:
				case FEATURES.RAILWAY:
				case FEATURES.ROAD:
				case FEATURES.SMOKE:
				case FEATURES.TRAIL:
					c = 0;
					break;
				case FEATURES.FENCE:
				case FEATURES.HEDGE:
				case FEATURES.WALL:
					c = 1;
					break;
				case FEATURES.CLIFF:
					throw new Map.ImpassableHexException("Cliff sides have no cost and can only be climbed with an Advance");
					
				default:
					throw new System.NotImplementedException(String.Format("The feature %s has no associated move cost", f.ToString()));
			}

			return c;
		}

		public static int TerrainCost(TERRAIN t) {
			int c = 0;

			switch (t) {
				case TERRAIN.OPEN:
				case TERRAIN.FIELD:
				case TERRAIN.ORCHARD:
					c = 1;
					break;
				case TERRAIN.BUILDING:
				case TERRAIN.BRUSH:
				case TERRAIN.GULLY:
				case TERRAIN.WOODS:
					c = 2;
					break;
				case TERRAIN.MARSH:
				case TERRAIN.STREAM:
					c = 3;
					break;
				case TERRAIN.BLAZE:
				case TERRAIN.MAJORBRIDGE:
				case TERRAIN.WATERBARRIER:
					throw new Map.ImpassableHexException(String.Format("The terrain %s is impassable",t.ToString()));
				default:
					throw new System.NotImplementedException(String.Format("The terrain %s has no associated move cost", t.ToString()));
			}

			//TODO: throw exception for impassable terrain

			return c;
		}

		public static int FeatureCover(FEATURES f) {
			int c = 0;

			switch (f) {
					
				case FEATURES.ROAD:
					c = -1;
					break;
				case FEATURES.NONE:
				case FEATURES.CLIFF:
				case FEATURES.FENCE:
				case FEATURES.RAILWAY:
				case FEATURES.SMOKE:
				case FEATURES.TRAIL:
					c = 0;
					break;
				case FEATURES.HEDGE:
					c = 1;
					break;
				case FEATURES.MINORBRIDGE:
				case FEATURES.WALL:
					c = 2;
					break;
				default:
					throw new System.NotImplementedException(String.Format("The feature %s has no associated cover", f.ToString()));

			}

			return c;
		}

		public static int TerrainCover(TERRAIN t) {
			int c = 0;

			switch (t) {
				case TERRAIN.STREAM:
					c = -1;
					break;
				case TERRAIN.FIELD:
				case TERRAIN.MARSH:
				case TERRAIN.OPEN:
					c = 0;
					break;
				case TERRAIN.BRUSH:
				case TERRAIN.GULLY:
				case TERRAIN.ORCHARD:
					c = 1;
					break;
				case TERRAIN.MAJORBRIDGE:
				case TERRAIN.WOODS:
					c = 2;
					break;
				case TERRAIN.BUILDING:
					c = 3;
					break;
				case TERRAIN.BLAZE:
				case TERRAIN.WATERBARRIER:
					throw new Map.ImpassableHexException(String.Format("Terrain is %s impassable and has no cover", t.ToString()));
				default:
					throw new System.NotImplementedException(String.Format("The terrain %s has no associated cover", t.ToString()));
			}

			return c;
		}

		public Map() {
			int c, r;
			hexes = new Hex[Map.Width,Map.Height];
			
			for (c = 0; c < Map.Width; c++) {
				for (r = 0; r < Map.Height; r++) {
					hexes[c, r] = new Hex(this, c, r);
				}
			}
			Objectives = new List<ObjectiveHexes>(5);
			for (c = 0; c < Objectives.Capacity; c++)
				Objectives.Add(new ObjectiveHexes(c + 1));
		}

		public static int Distance(Hex h1, Hex h2) {
			return HexCoordinates.Distance(h1.Coordinates, h2.Coordinates);
		}

		//Get a hex by number (0 = A1, 1 = A2, 2 = A3 ... 149 = O10)
		public Hex GetHex(int i) {
			return GetHex(i / Map.Height, i % Map.Height);
		}

		//Get a hex by its name as it appears on the map
		public Hex GetHex(String name) {
			Hex h = null;
			int col = (int)name[0] - (int)'A';
			int row;
			if (int.TryParse(name.Substring(1), out row)) {
				h = this.GetHex(col, row-1);
			}
			return h;
		}

		//Get a hex by column and row (0,0 = A1)
		public Hex GetHex(int col, int row) {
			Hex h = null;

			try {
				h = this.hexes[col, row];
			}
			catch(System.IndexOutOfRangeException) {
				throw new Map.OffBoardHexSelected(String.Format("Off-board Hex {0}{1} requested", col, row));
			}

			return h;
		}

		public Hex GetHex(HexCoordinates hc) {
			return GetHex(hc.Col, hc.Row);
		}

		//Get a hex based on a direction from another hex
		public Hex GetHex(Hex h, COMPASS direction, int distance = 1) {
			return GetHex(HexCoordinates.GetHexCoordinates(h, direction, distance));
		}

		public List<Hex> GetHexesWithin(Hex h1, int radius) {
			List<Hex> hexes = new List<Hex>();
			MetaHex mh = new MetaHex(h1, radius);
			
			List<HexCoordinates> hc = mh.GetAll(true);

			foreach (var c in hc) {
				try {
					hexes.Add(GetHex(c));
				}
				catch (Map.OffBoardHexSelected e) {

				}
			}

			return hexes;
		}

		public void Load(XDocument xDoc) {
			Load(xDoc.Root);
		}

		public void Load(XElement xml) {

			Hex hex, losHex;
			int objective;
			XAttribute attr;

			IEnumerable<IEnumerable<XElement>> hexesNodes =
				from el in xml.Descendants("hexes")
				select el.Elements();

			foreach (IEnumerable<XElement> hexesNode in hexesNodes) {
				foreach (XElement hexNode in hexesNode) {

					hex = GetHex(hexNode.Name.ToString());
					if (hex != null) {
						hex.SetTerrain(hexNode.Attribute("terrain").Value);

						SetFeatures(hex, "fence", hexNode);
						SetFeatures(hex, "road", hexNode);
						SetFeatures(hex, "hedge", hexNode);
						SetFeatures(hex, "trail", hexNode);

						attr = hexNode.Attribute("objective");
						if (attr != null && int.TryParse(attr.Value, out objective)) {
							hex.HexObjective = Objectives[objective-1];
							Objectives[objective-1].AddHex(hex);
						}

						//set up the LOS
						IEnumerable<XElement> losHexes =
							from h in hexNode.Elements()
							select h;

						foreach (XElement los in losHexes) {
							losHex = GetHex(los.Name.ToString());
							if (losHex != null) {

								hex.SetLOS(losHex, int.Parse(los.Value));
								losHex.SetLOS(hex, int.Parse(los.Value));

							}
						}

					}

				}
			}

		}

		public int MoveCost(Hex h1, COMPASS d) {
			int moveCost = 0;
			Hex h2 = this.GetHex(h1, d);
			FEATURES f = FEATURES.NONE;

			if (h2 != null) {

				try {
					f = h1.GetSideFeature(d);
					if (f == FEATURES.ROAD || f == FEATURES.TRAIL || f == FEATURES.RAILWAY)
						moveCost = 1;
					else if (h1.Terrain != TERRAIN.MAJORBRIDGE)
						moveCost = Map.TerrainCost(h2.Terrain) + Map.FeatureCost(f);
					else
						throw new Map.ImpassableHexException("Cannot leave a Major Bridge from a non-road or non-railway side");
				}
				catch (Map.ImpassableHexException E) {
					if (f == FEATURES.ROAD || f == FEATURES.RAILWAY) //major bridges
						moveCost = 1;
					else
						throw (E);
				}

				//penalty for uphill movement
				moveCost += Math.Max(0, (h2.Elevation - h1.Elevation));
					

			}
			else {
				throw new Map.OffBoardHexSelected(String.Format("The hex %s cannot be moved from in the direction of %s",h1.Name,d.ToString()));
				//TODO: if trying to move off map (and/or compute validity of off-map move)
			}
			return moveCost;
		}

		private void SetFeatures(Hex h, String f, XElement hNode) {
			XAttribute feature = hNode.Attribute(f);
			Hex neighbor;

			if (feature != null) {
				var sides = new List<COMPASS>(feature.Value.Split(',').Select(s => (COMPASS)Enum.Parse(typeof(COMPASS),s)-1));
				foreach (var side in sides) {
					h.SetSideFeature(side, f);
					COMPASS d = (COMPASS)Enum.Parse(typeof(COMPASS), side.ToString());

					try {
						neighbor = this.GetHex(h, (COMPASS)side);
						if (neighbor != null)
							neighbor.SetSideFeature((COMPASS)(side + 3), f);
					}
					catch (Map.OffBoardHexSelected) { }
				}
			}
		}

		/*
		 * 
		 * 
		 * CLASS HexCoordinates
		 * 
		 * 
		 */
		public class HexCoordinates {
			private int col;
			private int row;

			public HexCoordinates(int c, int r) {
				col = c;
				row = r;
			}
			public int Col {
				get { return col; }
				set { col = value; }
			}
			public int Row {
				get { return row; }
				set { row = value; }
			}


			public static int Distance(HexCoordinates h1, HexCoordinates h2) {
				int d = 0;
				HexCoordinates w, e;
				int neIntersect, seIntersect;

				if (h1 != null && h2 != null && h1 != h2) {
					if (h1.Col < h2.Col) {
						w = h1;
						e = h2;
					}
					else {
						w = h2;
						e = h1;
					}

					if (h1.Col == h2.Col) {
						d = Math.Abs(h1.Row - h2.Row);
					}
					else {
						//find the row we would intersect if we travelled straight northeast and straight southeast (may be an off-board row)
						neIntersect = w.Row - (w.Col % 2 == 0 ? (int)Math.Ceiling((double)(e.Col - w.Col) / 2) : (int)Math.Floor((double)(e.Col - w.Col) / 2));
						seIntersect = w.Row + (w.Col % 2 == 0 ? (int)Math.Floor((double)(e.Col - w.Col) / 2) : (int)Math.Ceiling((double)(e.Col - w.Col) / 2));

						d = e.Col - w.Col;
						//if the target hex falls between the ne and se intersections, the distance is simply the distance between columns
						//otherwise, we also need to add in the vertical offset remaining.
						if (e.Row < neIntersect)
							d += neIntersect - e.Row;
						if (e.Row > seIntersect)
							d += e.Row - seIntersect;
					}
				}

				return d;
			}


			public static HexCoordinates GetHexCoordinates(Hex h, COMPASS direction, int distance = 1) {
				return GetHexCoordinates(h.Coordinates, direction, distance);
			}

			//returns the coordinates of the hex asked for.  MAY BE A THEORETICAL OFFBOARD HEX (used for many purposes)
			public static HexCoordinates GetHexCoordinates(HexCoordinates h, COMPASS direction, int distance = 1) {
				HexCoordinates hc = new HexCoordinates(h.Col, h.Row);
				switch (direction) {

					case COMPASS.NORTH:
						hc.Row -= distance;
						break;

					case COMPASS.NORTHEAST:
						hc.Col += distance;
						hc.Row -= h.Col % 2 == 0 ? (int)Math.Ceiling(((double)distance) / 2) : (int)Math.Floor(((double)distance) / 2);
						break;

					case COMPASS.SOUTHEAST:
						hc.Col += distance;
						hc.Row += h.Col % 2 == 0 ? (int)Math.Floor(((double)distance) / 2) : (int)Math.Ceiling(((double)distance) / 2);
						break;

					case COMPASS.SOUTH:
						hc.Row += distance;
						break;

					case COMPASS.SOUTHWEST:
						hc.Col -= distance;
						hc.Row += h.Col % 2 == 0 ? (int)Math.Floor(((double)distance) / 2) : (int)Math.Ceiling(((double)distance) / 2);
						break;

					case COMPASS.NORTHWEST:
						hc.Col -= distance;
						hc.Row -= h.Col % 2 == 0 ? (int)Math.Ceiling(((double)distance) / 2) : (int)Math.Floor(((double)distance) / 2);
						break;
				}

				return hc;
			}

			public static bool OnMap(HexCoordinates hc) {
				return hc.Col < Map.Width && hc.Row < Map.Height && hc.Col >= 0 && hc.Row >= 0;
			}
		}


		/*
		 * 
		 * 
		 * 
		 * CLASS HEX
		 * 
		 * 
		 * 
		 */
		public class Hex {

			private HexCoordinates coordinates;
			private String name;

			private TERRAIN terrain;
			private FEATURES feature;
			private FEATURES[] sideFeatures;

			private int elevation;
			private ObjectiveHexes objective;

			private Stack stack;

			private Dictionary<Hex, int> LOS;

			private Map map;


			public Hex(Map map, HexCoordinates coordinates) : this(map, coordinates.Col, coordinates.Row){
				
			}

			public Hex(Map m, int col, int row) {

				this.map = m;
				this.coordinates = new HexCoordinates(col, row);
				
				this.name = String.Concat(new Object[] { (char)((int)'A' + col), 1 + row });

				//start the hex out as blank
				this.terrain = TERRAIN.OPEN;
				this.feature = FEATURES.NONE;
				this.sideFeatures = new FEATURES[6]; //implicit initialization

				this.stack = new Stack();
				this.LOS = new Dictionary<Hex, int>();
			}

			/*
			 * 
			 * GETTERS  AND SETTERS
			 * 
			 */
			public int Col {
				get { return coordinates.Col; }
				set { if (value < Map.Width) coordinates.Col = value; }
			}

			public HexCoordinates Coordinates {
				get { return coordinates; }
			}

			public int Cover() {
				//TODO: side features do not provide cover for all fire.
				//What really needs to happen is to check LOS to all enemy units 
				//and compute effective cover based on who can fire and from what
				//angle while taking into account mortar and artillery burst
				int c = TerrainCover(Terrain);
				c = Math.Max(c, FeatureCover(feature));
				foreach (COMPASS d in Enum.GetValues(typeof(COMPASS))) {
					c = Math.Max(c, FeatureCover(GetSideFeature(d)));
				}
				return c;
			}

			public int Elevation {
				get { return elevation; }
			}

			public ObjectiveHexes HexObjective {
				get { return objective; }
				set { objective = value; }
			}

			public int LeaderBonus() {
				int bonus = 0;
				foreach (var unit in stack) {
					if (unit.IsLeader() && unit.Leadership > bonus)
						bonus = unit.Leadership;
				}
				return bonus;
			}

			public String Name {
				get { return name; }
			}

			public int Row {
				get { return coordinates.Row; }
				set { if (value < Map.Height) coordinates.Row = value; }
			}

			public TERRAIN Terrain {
				get { return terrain; }
				set { this.terrain = value; }
			}
			
			public void SetTerrain(String terrain) {
				TERRAIN t = (TERRAIN)Enum.Parse(typeof(TERRAIN), terrain.ToUpper());
				if (Enum.IsDefined(typeof(TERRAIN), t))
					Terrain = t;
			}

			public FEATURES GetSideFeature(COMPASS side) {
				return this.sideFeatures[(int)side];
			}

			public void SetSideFeature(COMPASS side, String feature) {
				FEATURES f = (FEATURES)Enum.Parse(typeof(FEATURES), feature.ToUpper());
				if (Enum.IsDefined(typeof(FEATURES), f))
					this.SetSideFeature(side, f);
			}

			public void SetSideFeature(COMPASS side, FEATURES feature) {
				side = (COMPASS)((int)side % Enum.GetNames(typeof(COMPASS)).Length);
				this.sideFeatures[(int)side] = feature;
			}

			public void SetLOS(Hex h, int hindrance) {
				LOS[h] = hindrance;
			}

			public List<Hex> GetAllLOS() {
				return LOS.Keys.ToList();
			}

			public bool HasLOS(Hex h) {
				return LOS.ContainsKey(h);
			}

			public int GetHindrance(Hex h) {
				if (LOS.ContainsKey(h))
					return LOS[h];
				else 
					throw new NoLOSException(String.Format("No line of sight between {0} and {1}", this.Name, h.Name));
			}

			public List<Hex> GetHexesWithin(int r) {
				return map.GetHexesWithin(this, r);
			}
			/*
			 * OTHER FUNCTIONS
			 */
			public bool IsAccessible() {
				bool a = true;
				try {
					Map.TerrainCost(Terrain);
				}
				catch (Map.ImpassableHexException) {
					a = false;
				}
				return a;
			}

			public bool IsRoadHex() {
				bool r = false;
				foreach (COMPASS d in Enum.GetValues(typeof(COMPASS))) {
					r = r || GetSideFeature(d) == FEATURES.ROAD;
				}

				return r;
			}

            public Stack GetOccupants() {
                //TODO: return something that can't be manipulated. A clone or copy.
                return stack;
            }

            public bool IsOccupiedByFaction(FACTION f) {
                bool isOccupied = false;

                foreach (var unit in stack)
                {
                    if (unit.Nation.Faction == f) {
                        isOccupied = true;
                        break;
                    }
                }
                return isOccupied;
            }

		}

		/*
		 * 
		 * 
		 * 
		 * CLASS METAHEX
		 * 
		 * 
		 * 
		 */
		public class MetaHex {

			private HexCoordinates[] points = new HexCoordinates[6];
			private Hex center;

			private int radius;

			public MetaHex(Hex h1, int r) {
				center = h1;
				radius = r;
				foreach (COMPASS d in Enum.GetValues(typeof(COMPASS))) {
					points[(int)d] = HexCoordinates.GetHexCoordinates(center, d, r);
					
				}
			}

			public bool Contains(Hex h1) {
				return Contains(h1.Coordinates);
			}

			public bool Contains(HexCoordinates h1) {
				return (HexCoordinates.Distance(h1, points[(int)COMPASS.NORTH]) <= radius * 2
					&& HexCoordinates.Distance(h1, points[(int)COMPASS.SOUTHEAST]) <= radius * 2
					&& HexCoordinates.Distance(h1, points[(int)COMPASS.SOUTHWEST]) <= radius * 2);
			}

			public List<HexCoordinates> GetAll(bool bOnMapOnly) {
				List<HexCoordinates> hexes = new List<HexCoordinates>();

				HexCoordinates nw = points[(int)COMPASS.NORTH];
				HexCoordinates ne = nw;

				HexCoordinates h, topHex;

				for (int x = (radius * 2) + 1; x > radius; x--) {
					for (int y = 0; y < x; y++) {
						topHex = nw;
						do {
							h = HexCoordinates.GetHexCoordinates(topHex, COMPASS.SOUTH, y);
							
							if (!bOnMapOnly || HexCoordinates.OnMap(h))
								hexes.Add(h);

							if (topHex == nw && nw != ne)
								topHex = ne;
							else
								topHex = null;
						} while (topHex != null);
						
					}

					nw = HexCoordinates.GetHexCoordinates(nw, COMPASS.SOUTHWEST, 1);
					ne = HexCoordinates.GetHexCoordinates(ne, COMPASS.SOUTHEAST, 1);
				}

				return hexes;
			}
		}


		/*
		 * 
		 * 
		 * CLASS HEXORDERER
		 * 
		 * 
		 */
		public class HexOrderer : Comparer<Hex> {
			public HexOrderer(){}

			public override int Compare(Hex x, Hex y) {
				if (x.Col == y.Col) {
					return x.Row.CompareTo(y.Row);
				}
				return x.Col.CompareTo(y.Col);
			}
		}


		/*
		 * 
		 * 
		 * CLASS OBJECTIVE
		 * 
		 * 
		 */

		//This class describes only the location of each of the 5 labeled objectives on the map
		//It does NOT describe the owner or the point value. That stuff is left to the gameManager and/or the gameboard classes
		public class ObjectiveHexes {

			private int _number;
			private HashSet<Hex> hexes;
            
			public ObjectiveHexes(int num) : this(num, Enumerable.Empty<Hex>()){
			}

			public ObjectiveHexes(int num, IEnumerable<Hex> collection) {
				_number = num;
				foreach (Hex h in collection)
					AddHex(h);
			}

			public int Number {
				get { return _number; }
				set { _number = value; }
			}

            public HashSet<Hex> Hexes {
                get { return hexes; }
            }

			public void AddHex(Hex h) {
				if(hexes == null)
					hexes = new HashSet<Hex>();
				hexes.Add(h);
			}

		}

		/*
		 * 
		 * Map-based Exceptions
		 * 
		 * 
		 */
		public class ImpassableHexException : System.ApplicationException {
			public ImpassableHexException() : base() { }
			public ImpassableHexException(string message) : base(message) { }
			public ImpassableHexException(string message, System.Exception inner) : base(message, inner) { }
		}

		public class OffBoardHexSelected : System.ApplicationException {
			public OffBoardHexSelected() : base() { }
			public OffBoardHexSelected(string message) : base(message) { }
			public OffBoardHexSelected(string message, System.Exception inner) : base(message, inner) { }
		}

		public class NoLOSException : System.ApplicationException {
			public NoLOSException() : base() { }
			public NoLOSException(string message) : base(message) { }
			public NoLOSException(string message, System.Exception inner) : base(message, inner) { }
		}

	}

}

