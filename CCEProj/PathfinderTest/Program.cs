using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CW_Utils;
using System.Diagnostics;

namespace CombatCommander {
	class Program {

		static Map.Hex moveRandom(Unit u, Map m) {
			Map.Hex result = null;
			Map.Hex candidate = null;

			while (result == null) {
				candidate = m.GetHex(Randomizer.get(0, 149));
				if (candidate.CanBeMovedInto(u.Nation.PlayedBy))
					result = candidate;
			}
			return result;
		}

		static void Main(string[] args) {

			Map myMap = new Map();
			Scenario scenario = new Scenario(myMap);
			Game myGame = new Game(scenario);

			Player player1 = new Player(myGame, Russian.Instance);
			Player player2 = new Player(myGame, German.Instance);

			XDocument map1 = XDocument.Load("C:\\users\\a3skszz\\Documents\\Chad\\CCE\\map1.xml");

			myMap.Load(map1);

			Pathfinder pf = new Pathfinder();

			string h1, h2;
			double sw, cw, ow;
			Map.Hex target, target2;

			Stopwatch stop = new Stopwatch();
			TimeSpan ts;
			string elapsedTime;

			stop.Start();

			List<HexInfo> path, path2;
			ObjectiveChit c;

			c = player1.DrawObjective();
			c = player2.DrawObjective();
			c = myGame.DrawOpenObjective();

			Russian.PvtGelon l1 = new Russian.PvtGelon();
			Russian.PvtGelon l2 = new Russian.PvtGelon();


			Russian.Assault squad = new Russian.Assault();
			//squad.Break();



			German.Pioneer enemy = new German.Pioneer();
			enemy.Mover = new HexInfoCompare(1, 1, 0, 2);

			moveRandom(enemy, myMap).MoveInto(enemy);
			moveRandom(l1, myMap).MoveInto(l1);
			moveRandom(l2, myMap).MoveInto(l2);
			moveRandom(squad, myMap).MoveInto(squad);

			Console.WriteLine(String.Format("Enemy: {0}", enemy.location.Name));
			Console.WriteLine(String.Format("Leader 1: {0}", l1.location.Name));
			Console.WriteLine(String.Format("Leader 2: {0}", l2.location.Name));
			Console.WriteLine(String.Format("Squad: {0}", squad.location.Name));

			Console.Write("Speed:");
			sw = Double.Parse(Console.ReadLine());
			Console.Write("Cover:");
			cw = Double.Parse(Console.ReadLine());
			Console.Write("Objectives:");
			ow = Double.Parse(Console.ReadLine());

			squad.Mover = new HexInfoCompare(1, sw, cw, ow);
			target = enemy.location;
			target2 = squad.location;

			do {

				if (enemy.location == target) {
					target = moveRandom(enemy, myMap);
					Console.WriteLine(String.Format("\r\nNEW Enemy Objective: {0}", target.Name));
				}
				if (squad.location == target2) {
					target2 = moveRandom(squad, myMap);
					Console.WriteLine("\r\nNEWSquad Objective: {0}", target2.Name);
				}


				while (enemy.location != target && squad.location != target2) {
					if (enemy.location != target) {
						path = pf.FindPath(enemy, myMap, enemy.location, target);
						IEnumerable<HexInfo> firstTurn = path.TakeWhile(p => p.turnCount == 1);
						Console.Write("\r\nEnemy:");
						foreach (HexInfo hi in firstTurn) {
							if (hi.mapHex != enemy.location) {
								hi.mapHex.MoveInto(enemy);
								Console.Write(String.Format("=>{0}", hi.mapHex.Name));
							}
						}
					}

					if (squad.location != target2) {
						path = pf.FindPath(squad, myMap, squad.location, target2);
						IEnumerable<HexInfo> firstTurn = path.TakeWhile(p => p.turnCount == 1);
						Console.Write("\r\nSquad:");
						foreach (HexInfo hi in firstTurn) {
							if (hi.mapHex != squad.location) {
								hi.mapHex.MoveInto(squad);
								Console.Write(String.Format("=>{0}", hi.mapHex.Name));
							}
						}
					}
				}

				Console.Write("\r\nContinue? ");

			} while (Console.ReadKey().KeyChar == 'y');



			/*
						while (h1 != "q") {
							try {

								Console.Clear();

								squad.Mover = new HexInfoCompare(1, sw, cw, ow);

								Map.Hex start = myMap.GetHex(h1);
								Map.Hex end = myMap.GetHex(h2);

								int turn = 0;

								while (start != end) {
									path = pf.FindPath(squad, myMap, start, end);
									IEnumerable<HexInfo> firstTurn = path.TakeWhile(p => p.turnCount == 1);
									foreach(var p in firstTurn) {
										Console.WriteLine("{0} (Turn: {1}; Exp: {2}; BONUS: {3})", p.mapHex.Name, turn, p.terrainCost, p.roadBonusUsed);
										start = p.mapHex;
										start.MoveInto(squad);
									}
									turn++;
									Console.WriteLine("");
								}

					
								Console.WriteLine();
								Console.WriteLine();

								Console.Write("Hex 1:");
								h1 = Console.ReadLine().ToUpper();
								Console.Write("Hex 2:");
								h2 = Console.ReadLine().ToUpper();
								Console.Write("Speed:");
								sw = Double.Parse(Console.ReadLine());
								Console.Write("Cover:");
								cw = Double.Parse(Console.ReadLine());
								Console.Write("Objectives:");
								ow = Double.Parse(Console.ReadLine());
							}
							catch (Exception E) {
								break;
							}
						}*/

		}
	}
}
