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
			XDocument map1 = XDocument.Load("C:\\users\\a3skszz\\Documents\\Chad\\CCE\\map1.xml");

			myMap.Load(map1);

			Map.Hex h1, h2;

			try {
				Console.Write("Hex 1: ");
				h1 = myMap.GetHex(Console.ReadLine());
				Console.Write("Hex 2: ");
				h2 = myMap.GetHex(Console.ReadLine());

				while (h1 != null && h2 != null) {
					Console.WriteLine(h1.GetHindrance(h2));
					Console.WriteLine();

					Console.Write("Hex 1: ");
					h1 = myMap.GetHex(Console.ReadLine());
					Console.Write("Hex 2: ");
					h2 = myMap.GetHex(Console.ReadLine());
				}

			}
			catch(Exception e) {

			}

		}
	}
}
