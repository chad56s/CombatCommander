using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CW_Utils;

namespace CombatCommander {

	public class Pathfinder {

		
		HexInfo[,] hexesVisited;
		bool bDirty;

		public Pathfinder() {
			this.Reset();
			bDirty = false;
		}

		public List<HexInfo> DebugPath(Unit u, Map m, Map.Hex h1, Map.Hex h2) {
			List<HexInfo> path = FindPath(u, m, h1, h2);

			Console.Clear();

			foreach (var h in hexesVisited) {
				string hex = h.mapHex == null ? null : h.mapHex.Name;
				string prevHex = h.previousHex == null ? "" : h.previousHex.mapHex == null ? "" : h.previousHex.mapHex.Name;
				if(hex != null)
					Console.WriteLine("{0}: Cost:{1}; Distance:{2}; Previous:{3}; Visited:{4}", hex, h.terrainCost, h.distanceFromTarget, prevHex, h.visited); 
			}

			return path;
		}

		public List<HexInfo> FindPath(Unit unit, Map m, Map.Hex h1, Map.Hex h2) {

			List<HexInfo> p = new List<HexInfo>();
			
			if (!h1.IsAccessible())
				return p;

			//TODO: reset after every move to allow for retracing steps (e.g. to duck into cover at the end of a move and get back out on the road the next turn?)
			if (this.bDirty)
				this.Reset();
			this.bDirty = true;

			HexInfo startHex = new HexInfo(); 
			HexInfo currentHex = startHex;
			Map.Hex destination = h2;
			HexInfo existingScore;
			HexInfo closest;

			HexInfoCompare comparer = unit.Mover;
			MinHeap<HexInfo> priorityQueue = new MinHeap<HexInfo>(comparer);

			int mp, moveCost;
			int ties =  0;

			bool giveUp = false;

			this.hexesVisited[h1.Col, h1.Row] = startHex;

			startHex.terrainCost = 0;
			startHex.distanceFromTarget = Map.Distance(h1, h2);
			startHex.mapHex = h1;

			closest = startHex;

			while (currentHex.mapHex != destination && !giveUp) {
				h1 = currentHex.mapHex;

				//TODO: when stacks are implemented, mp = printedMovement + better of h1 leader and stack's leader
				mp = unit.PrintedMovement + h1.LeaderBonus();

				foreach (COMPASS d in Enum.GetValues(typeof(COMPASS))) {
					try {
						h2 = m.GetHex(h1, d);

						if(h2.CanBeMovedInto(unit.Nation.PlayedBy)) {
							existingScore = this.hexesVisited[h2.Col, h2.Row];

							moveCost = (int)m.MoveCost(h1, d);
						
							//don't compute a score if the unit doesn't even have enough MP to get in
							if (mp >= moveCost && (existingScore == null || !existingScore.visited)) {

								HexInfo newScore = new HexInfo();

								bool thisTurn = currentHex.mpsSpent + moveCost <= mp + (currentHex.roadBonusUsed ? 1 : 0);

								newScore.distanceFromTarget = Map.Distance(h2, destination);
								newScore.turnCount = thisTurn ? currentHex.turnCount : currentHex.turnCount + 1;
							
								//newScore.terrainCost = thisTurn ? currentHex.terrainCost + (int)moveCost : (int)moveCost;
								//experiment
								newScore.terrainCost = (int)moveCost;
								newScore.mpsSpent = thisTurn ? currentHex.mpsSpent + moveCost : moveCost;
								newScore.roadBonusUsed = thisTurn ? currentHex.roadBonusUsed || h2.IsRoadHex() : h2.IsRoadHex();
								newScore.previousHex = currentHex;
								newScore.mapHex = h2;
								newScore.score = comparer.Compute(newScore, unit.Nation.PlayedBy);
							
								if (existingScore == null) {
									this.hexesVisited[h2.Col, h2.Row] = newScore;
									priorityQueue.Add(newScore);

									//update which hex is closest if needed
									if (newScore.distanceFromTarget < closest.distanceFromTarget) {
										closest = newScore;
										ties = 0;
									}
									else if (newScore.distanceFromTarget == closest.distanceFromTarget) {
										if (Randomizer.get(0, ++ties) == 0)
											closest = newScore;
									}
								}
								else if (comparer.Compare(newScore, existingScore) < 0) {
									//newScore is better than existingScore.  Replace
									this.hexesVisited[h2.Col, h2.Row] = newScore;
									priorityQueue.Replace(existingScore, newScore);
								}

							}
						}
					}
					catch (Map.OffBoardHexSelected) { }
					catch (Map.ImpassableHexException) { }
				}
				currentHex.visited = true;

				if (priorityQueue.Count > 0)
					currentHex = priorityQueue.ExtractDominating();
				else 
					giveUp = true;

			}

			//TODO: give the option to return null if no path.
			
			//if we couldn't find a path, return the closest hex
			if (giveUp) {
				currentHex = closest;

				//if it's not a big enough change...return an empty path
				if (startHex.distanceFromTarget - currentHex.distanceFromTarget < comparer.findClosest)
					currentHex = startHex;
			}

			while (currentHex != null) {
				p.Insert(0, currentHex);
				currentHex = currentHex.previousHex;
			}

			return p;
		}

		private void Reset() {
			this.hexesVisited = new HexInfo[Map.Width, Map.Height];
			
			this.bDirty = false;
		}
	}


	public class HexInfo {

		public int terrainCost { get; set; }
		public int distanceFromTarget { get; set; }
		public int turnCount { get; set; }
		public int mpsSpent { get; set; }
		public bool roadBonusUsed { get; set; }
		public HexInfo previousHex { get; set; }
		public Map.Hex mapHex { get; set; }
		public bool visited { get; set; }

		public double score { get; set; }

		public HexInfo() {
			this.turnCount = 1;
			this.terrainCost = -1;
			this.distanceFromTarget = -1;
			this.mpsSpent = 0;
			this.roadBonusUsed = false;

			this.previousHex = null;
			this.mapHex = null;
			this.visited = false;

			this.score = 0;

		}

	}
	/*
	 *	This class should be used to compare hexes for varieties of reasons including, but not limited to
	 *	
	 *	Movement - path finding
	 *	Movement - destination determination
	 *	Set up
	 *	finding enemies
	 *	avoiding enemies
	 *	determining game objectives
	 *	
	 *  The goal is to make this objects of this class candidates for genetic mutation
	 */
	public class HexInfoCompare : Comparer<HexInfo> {

		public int findClosest { get; set; } //if no path exists, it must be able to get this much closer to settle for closest
		public double speedWeight { get; set; } //weight given to hexes based on speed of travel
		public double coverWeight { get; set; } //weight given to hexes with better cover
		public double objectivesWeight { get; set; } //weight given to go through hexes with objectives
		public double tw = 0.5;

		public double K = 0;
		public double C = 4;

		public HexInfoCompare() {
			findClosest = 0;
			speedWeight = 0;
			coverWeight = 0;
			objectivesWeight = 0;
		}

		public HexInfoCompare(int a, double b, double c, double d) {
			findClosest = a;
			speedWeight = b;
			coverWeight = c;
			objectivesWeight = d;
		}

		// TODO: going to have to figure out a different way to Compare for different reasons...abstracts, and inheritance and so forth
		public override int Compare(HexInfo x, HexInfo y) {
			int res = 0;
			
			res = x.score.CompareTo(y.score);

			if (res == 0) {
				//TODO: this really only applies to speedy movement and not to comparison purposes for cover, etc.  See the above TODO
				res = y.roadBonusUsed.CompareTo(x.roadBonusUsed);
			}

			return res;

		}

		public double Compute(HexInfo h, Commander p) {

			//the lower the better for moving
			double score = 0;
			//normalize the weights
			double totalOfWeights = speedWeight + coverWeight + objectivesWeight;
			double sw = speedWeight / totalOfWeights;
			double cw = coverWeight / totalOfWeights;
			double ow = objectivesWeight / totalOfWeights;
			
			
			Map.Objective obj = h.mapHex.HexObjective;
			int pointSwing = h.mapHex.GetPointSwing(p, true);
			if (pointSwing != 0) {
				//determine whether the point swing has already been accounted for previously on the path
				for (var prev = h.previousHex; prev != null; prev = prev.previousHex) {
					if (prev.mapHex.HexObjective == obj)
						pointSwing = 0;
				}
			}

			//TODO: factor in road bonus
			score = (Math.Log(h.turnCount) * tw) + ((sw * (h.terrainCost + h.distanceFromTarget)) + cw * (C - h.mapHex.Cover())) - ow * (pointSwing) + K;
			
			if (h.previousHex != null) {
				score += h.previousHex.score;
				//remove the predicted element of the previous hex from this hex's score.
				score -= sw * h.previousHex.distanceFromTarget;
			}
			return score;
		}

	}

}
