using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CombatCommander;

using System.Xml;
using System.Xml.Linq;
using CW_Utils;
using System.Diagnostics;

namespace LOS {
	class LOSProgram {

		static void Main(string[] args) {

			Map myMap = new Map();
			XDocument map1 = XDocument.Load("..\\..\\..\\CCE\\map1.xml");

			myMap.Load(map1);

			Map.Hex h1, h2;
			Russian.GuardsRifle squad = new Russian.GuardsRifle();
			Russian.PvtGelon leader = new Russian.PvtGelon();


			try {

				Console.Write("Hex 1: ");
				h1 = myMap.GetHex(Console.ReadLine().ToUpper());

				while (h1 != null) {
					h1.MoveInto(squad);
					h1.MoveInto(leader);
					Console.Clear();
					IEnumerable<Map.Hex> losHexes = squad.GetFireableHexes();
					foreach (var h in losHexes)
						Console.WriteLine(String.Format("{0}: {1}", h.Name, h1.GetHindrance(h)));
					
					Console.WriteLine();
					Console.Write("Hex 1: ");
					h1 = myMap.GetHex(Console.ReadLine().ToUpper());
				}

			}
			catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.ReadKey();
			}

		}
	}
}
